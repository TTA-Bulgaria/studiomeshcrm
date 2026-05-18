using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Crm.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/facebook/oauth")]
[EnableRateLimiting("facebook-oauth")]
public class FacebookOAuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;
    private readonly IGenericRepository<ProjectAdAccount> _adAccountRepo;
    private readonly ICurrentUserContext _userContext;
    private readonly ITokenEncryptionService _encryption;
    private readonly HttpClient _httpClient;

    public FacebookOAuthController(
        IConfiguration config,
        AppDbContext db,
        IGenericRepository<ProjectAdAccount> adAccountRepo,
        ICurrentUserContext userContext,
        ITokenEncryptionService encryption,
        IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _db = db;
        _adAccountRepo = adAccountRepo;
        _userContext = userContext;
        _encryption = encryption;
        _httpClient = httpClientFactory.CreateClient();
    }

    [Authorize]
    [HttpGet("connect")]
    public async Task<IActionResult> Connect([FromQuery] Guid projectId)
    {
        var appId = _config["MetaAds:AppId"];
        if (string.IsNullOrEmpty(appId))
            return BadRequest(new { message = "Meta App ID is not configured on this server." });

        await PurgeExpiredAsync();

        var stateKey = Guid.NewGuid().ToString("N");
        _db.FacebookOAuthSessions.Add(new FacebookOAuthSession
        {
            Key = stateKey,
            Phase = FacebookOAuthPhase.State,
            ProjectId = projectId,
            TenantId = _userContext.TenantId ?? Guid.Empty,
            ExpiresAt = DateTime.UtcNow.AddMinutes(20)
        });
        await _db.SaveChangesAsync();

        var redirectUri = Uri.EscapeDataString(CallbackUri);
        var scopes = Uri.EscapeDataString("ads_read,ads_management");
        var facebookUrl = $"https://www.facebook.com/v19.0/dialog/oauth" +
                          $"?client_id={appId}" +
                          $"&redirect_uri={redirectUri}" +
                          $"&state={stateKey}" +
                          $"&scope={scopes}";

        return Redirect(facebookUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state, [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
            return Redirect($"{FrontendUrl}/integrations?error=facebook_denied");

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            return Redirect($"{FrontendUrl}/integrations?error=invalid_request");

        var oauthState = await _db.FacebookOAuthSessions
            .FirstOrDefaultAsync(s => s.Key == state && s.Phase == FacebookOAuthPhase.State);

        if (oauthState == null || oauthState.ExpiresAt < DateTime.UtcNow)
        {
            if (oauthState != null) _db.FacebookOAuthSessions.Remove(oauthState);
            await _db.SaveChangesAsync();
            return Redirect($"{FrontendUrl}/integrations?error=session_expired");
        }

        _db.FacebookOAuthSessions.Remove(oauthState);

        var appId = _config["MetaAds:AppId"];
        var appSecret = _config["MetaAds:AppSecret"];

        try
        {
            var shortTokenUrl = $"https://graph.facebook.com/v19.0/oauth/access_token" +
                                $"?client_id={appId}" +
                                $"&redirect_uri={Uri.EscapeDataString(CallbackUri)}" +
                                $"&client_secret={appSecret}" +
                                $"&code={code}";

            var shortTokenResponse = await _httpClient.GetFromJsonAsync<FacebookTokenResponse>(shortTokenUrl);
            if (shortTokenResponse?.AccessToken == null)
                return Redirect($"{FrontendUrl}/integrations?error=token_exchange_failed");

            var longTokenUrl = $"https://graph.facebook.com/v19.0/oauth/access_token" +
                               $"?grant_type=fb_exchange_token" +
                               $"&client_id={appId}" +
                               $"&client_secret={appSecret}" +
                               $"&fb_exchange_token={shortTokenResponse.AccessToken}";

            var longTokenResponse = await _httpClient.GetFromJsonAsync<FacebookTokenResponse>(longTokenUrl);
            var longLivedToken = longTokenResponse?.AccessToken ?? shortTokenResponse.AccessToken;
            var tokenExpiresAt = longTokenResponse?.ExpiresIn.HasValue == true
                ? DateTime.UtcNow.AddSeconds(longTokenResponse.ExpiresIn.Value)
                : DateTime.UtcNow.AddDays(60);

            var accountsUrl = $"https://graph.facebook.com/v19.0/me/adaccounts" +
                              $"?fields=id,name,account_status" +
                              $"&access_token={longLivedToken}";

            var accountsResponse = await _httpClient.GetFromJsonAsync<FacebookAdAccountsResponse>(accountsUrl);
            var adAccounts = accountsResponse?.Data ?? new List<FacebookAdAccount>();

            var sessionKey = Guid.NewGuid().ToString("N");
            _db.FacebookOAuthSessions.Add(new FacebookOAuthSession
            {
                Key = sessionKey,
                Phase = FacebookOAuthPhase.Session,
                ProjectId = oauthState.ProjectId,
                TenantId = oauthState.TenantId,
                LongLivedToken = longLivedToken,
                TokenExpiresAt = tokenExpiresAt,
                AdAccountsJson = JsonSerializer.Serialize(adAccounts),
                ExpiresAt = DateTime.UtcNow.AddMinutes(25)
            });
            await _db.SaveChangesAsync();

            return Redirect($"{FrontendUrl}/integrations/connect?session={sessionKey}");
        }
        catch
        {
            return Redirect($"{FrontendUrl}/integrations?error=token_exchange_failed");
        }
    }

    [Authorize]
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts([FromQuery] string session)
    {
        var oauthSession = await _db.FacebookOAuthSessions
            .FirstOrDefaultAsync(s => s.Key == session && s.Phase == FacebookOAuthPhase.Session);

        if (oauthSession == null || oauthSession.ExpiresAt < DateTime.UtcNow)
            return NotFound(new { message = "Session not found or expired. Please reconnect." });

        if (oauthSession.TenantId != (_userContext.TenantId ?? Guid.Empty))
            return NotFound(new { message = "Session not found or expired. Please reconnect." });

        var accounts = JsonSerializer.Deserialize<List<FacebookAdAccount>>(oauthSession.AdAccountsJson ?? "[]")
                       ?? new List<FacebookAdAccount>();

        return Ok(new
        {
            projectId = oauthSession.ProjectId,
            accounts = accounts.Select(a => new
            {
                id = a.Id,
                name = a.Name,
                isActive = a.AccountStatus == 1
            })
        });
    }

    [Authorize]
    [HttpPost("link")]
    public async Task<IActionResult> LinkAccounts([FromBody] LinkAccountsRequest request)
    {
        var oauthSession = await _db.FacebookOAuthSessions
            .FirstOrDefaultAsync(s => s.Key == request.Session && s.Phase == FacebookOAuthPhase.Session);

        if (oauthSession == null || oauthSession.ExpiresAt < DateTime.UtcNow)
            return BadRequest(new { message = "Session expired. Please reconnect Facebook." });

        if (oauthSession.TenantId != (_userContext.TenantId ?? Guid.Empty))
            return BadRequest(new { message = "Session expired. Please reconnect Facebook." });

        _db.FacebookOAuthSessions.Remove(oauthSession);

        var allAccounts = JsonSerializer.Deserialize<List<FacebookAdAccount>>(oauthSession.AdAccountsJson ?? "[]")
                          ?? new List<FacebookAdAccount>();

        var selectedAccounts = allAccounts
            .Where(a => request.SelectedAccountIds.Contains(a.Id))
            .ToList();

        if (!selectedAccounts.Any())
            return BadRequest(new { message = "No accounts selected." });

        var encryptedToken = _encryption.Encrypt(oauthSession.LongLivedToken ?? string.Empty);
        var linked = 0;

        foreach (var account in selectedAccounts)
        {
            var existing = await _adAccountRepo.AsQueryable()
                .FirstOrDefaultAsync(a => a.ExternalAccountId == account.Id && a.ProjectId == oauthSession.ProjectId);

            if (existing != null)
            {
                existing.AccessToken = encryptedToken;
                existing.TokenExpiresAt = oauthSession.TokenExpiresAt;
                existing.IsActive = true;
                await _adAccountRepo.UpdateAsync(existing);
            }
            else
            {
                await _adAccountRepo.AddAsync(new ProjectAdAccount
                {
                    Id = Guid.NewGuid(),
                    ProjectId = oauthSession.ProjectId,
                    Platform = AdPlatform.Meta,
                    ExternalAccountId = account.Id,
                    AccessToken = encryptedToken,
                    TokenExpiresAt = oauthSession.TokenExpiresAt,
                    IsActive = true,
                    TenantId = oauthSession.TenantId
                });
            }

            linked++;
        }

        await _adAccountRepo.SaveChangesAsync();
        await _db.SaveChangesAsync();
        return Ok(new { linked, projectId = oauthSession.ProjectId });
    }

    private async Task PurgeExpiredAsync()
    {
        var expired = await _db.FacebookOAuthSessions
            .Where(s => s.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
        if (expired.Count > 0)
        {
            _db.FacebookOAuthSessions.RemoveRange(expired);
            await _db.SaveChangesAsync();
        }
    }

    private string CallbackUri =>
        $"{_config["AppUrl"] ?? throw new InvalidOperationException("AppUrl is not configured.")}/api/facebook/oauth/callback";
    private string FrontendUrl =>
        _config["AppUrl"] ?? throw new InvalidOperationException("AppUrl is not configured.");
}

// ── Supporting types ────────────────────────────────────────────────────────

internal record FacebookTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; init; }
}

internal record FacebookAdAccountsResponse
{
    [JsonPropertyName("data")]
    public List<FacebookAdAccount>? Data { get; init; }
}

internal record FacebookAdAccount
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("account_status")]
    public int AccountStatus { get; init; }
}

public record LinkAccountsRequest
{
    public string Session { get; init; } = string.Empty;
    public List<string> SelectedAccountIds { get; init; } = new();
}
