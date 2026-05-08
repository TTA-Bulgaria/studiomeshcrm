using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/facebook/oauth")]
public class FacebookOAuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;
    private readonly IGenericRepository<ProjectAdAccount> _adAccountRepo;
    private readonly ICurrentUserContext _userContext;
    private readonly HttpClient _httpClient;

    public FacebookOAuthController(
        IConfiguration config,
        IMemoryCache cache,
        IGenericRepository<ProjectAdAccount> adAccountRepo,
        ICurrentUserContext userContext,
        IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _cache = cache;
        _adAccountRepo = adAccountRepo;
        _userContext = userContext;
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Step 1: Redirect the user to Facebook's OAuth consent screen.
    /// Navigate the browser directly to this URL (not as a fetch).
    /// </summary>
    [Authorize]
    [HttpGet("connect")]
    public IActionResult Connect([FromQuery] Guid projectId)
    {
        var appId = _config["MetaAds:AppId"];
        if (string.IsNullOrEmpty(appId))
            return BadRequest(new { message = "Meta App ID is not configured on this server." });

        var stateKey = Guid.NewGuid().ToString("N");
        _cache.Set($"fb_oauth_{stateKey}", new FacebookOAuthState
        {
            ProjectId = projectId,
            TenantId = _userContext.TenantId ?? Guid.Empty
        }, TimeSpan.FromMinutes(10));

        var redirectUri = Uri.EscapeDataString(CallbackUri);
        var scopes = Uri.EscapeDataString("ads_read,ads_management");
        var facebookUrl = $"https://www.facebook.com/v19.0/dialog/oauth" +
                          $"?client_id={appId}" +
                          $"&redirect_uri={redirectUri}" +
                          $"&state={stateKey}" +
                          $"&scope={scopes}";

        return Redirect(facebookUrl);
    }

    /// <summary>
    /// Step 2: Facebook redirects here after the user grants permission.
    /// Exchanges the code for a long-lived token, fetches ad accounts,
    /// then redirects the browser to the frontend account selector page.
    /// </summary>
    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state, [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
            return Redirect($"{FrontendUrl}/integrations?error=facebook_denied");

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            return Redirect($"{FrontendUrl}/integrations?error=invalid_request");

        if (!_cache.TryGetValue($"fb_oauth_{state}", out FacebookOAuthState? oauthState) || oauthState == null)
            return Redirect($"{FrontendUrl}/integrations?error=session_expired");

        _cache.Remove($"fb_oauth_{state}");

        var appId = _config["MetaAds:AppId"];
        var appSecret = _config["MetaAds:AppSecret"];

        try
        {
            // Exchange code → short-lived token
            var shortTokenUrl = $"https://graph.facebook.com/v19.0/oauth/access_token" +
                                $"?client_id={appId}" +
                                $"&redirect_uri={Uri.EscapeDataString(CallbackUri)}" +
                                $"&client_secret={appSecret}" +
                                $"&code={code}";

            var shortTokenResponse = await _httpClient.GetFromJsonAsync<FacebookTokenResponse>(shortTokenUrl);
            if (shortTokenResponse?.AccessToken == null)
                return Redirect($"{FrontendUrl}/integrations?error=token_exchange_failed");

            // Exchange short-lived → long-lived token (~60 days)
            var longTokenUrl = $"https://graph.facebook.com/v19.0/oauth/access_token" +
                               $"?grant_type=fb_exchange_token" +
                               $"&client_id={appId}" +
                               $"&client_secret={appSecret}" +
                               $"&fb_exchange_token={shortTokenResponse.AccessToken}";

            var longTokenResponse = await _httpClient.GetFromJsonAsync<FacebookTokenResponse>(longTokenUrl);
            var longLivedToken = longTokenResponse?.AccessToken ?? shortTokenResponse.AccessToken;

            // Fetch ad accounts this user has access to
            var accountsUrl = $"https://graph.facebook.com/v19.0/me/adaccounts" +
                              $"?fields=id,name,account_status" +
                              $"&access_token={longLivedToken}";

            var accountsResponse = await _httpClient.GetFromJsonAsync<FacebookAdAccountsResponse>(accountsUrl);
            var adAccounts = accountsResponse?.Data ?? new List<FacebookAdAccount>();

            var sessionKey = Guid.NewGuid().ToString("N");
            _cache.Set($"fb_session_{sessionKey}", new FacebookOAuthSession
            {
                ProjectId = oauthState.ProjectId,
                TenantId = oauthState.TenantId,
                LongLivedToken = longLivedToken,
                AdAccounts = adAccounts
            }, TimeSpan.FromMinutes(15));

            return Redirect($"{FrontendUrl}/integrations/connect?session={sessionKey}");
        }
        catch
        {
            return Redirect($"{FrontendUrl}/integrations?error=token_exchange_failed");
        }
    }

    /// <summary>
    /// Step 3: Frontend calls this to get the list of ad accounts from the OAuth session.
    /// </summary>
    [Authorize]
    [HttpGet("accounts")]
    public IActionResult GetAccounts([FromQuery] string session)
    {
        if (!_cache.TryGetValue($"fb_session_{session}", out FacebookOAuthSession? oauthSession) || oauthSession == null)
            return NotFound(new { message = "Session not found or expired. Please reconnect." });

        return Ok(new
        {
            projectId = oauthSession.ProjectId,
            accounts = oauthSession.AdAccounts.Select(a => new
            {
                id = a.Id,
                name = a.Name,
                isActive = a.AccountStatus == 1
            })
        });
    }

    /// <summary>
    /// Step 4: Frontend posts the user's selected accounts to save them.
    /// </summary>
    [Authorize]
    [HttpPost("link")]
    public async Task<IActionResult> LinkAccounts([FromBody] LinkAccountsRequest request)
    {
        if (!_cache.TryGetValue($"fb_session_{request.Session}", out FacebookOAuthSession? oauthSession) || oauthSession == null)
            return BadRequest(new { message = "Session expired. Please reconnect Facebook." });

        _cache.Remove($"fb_session_{request.Session}");

        var selectedAccounts = oauthSession.AdAccounts
            .Where(a => request.SelectedAccountIds.Contains(a.Id))
            .ToList();

        if (!selectedAccounts.Any())
            return BadRequest(new { message = "No accounts selected." });

        var allAccounts = await _adAccountRepo.GetAllAsync();
        var linked = 0;

        foreach (var account in selectedAccounts)
        {
            var existing = allAccounts.FirstOrDefault(a =>
                a.ExternalAccountId == account.Id && a.ProjectId == oauthSession.ProjectId);

            if (existing != null)
            {
                existing.AccessToken = oauthSession.LongLivedToken;
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
                    AccessToken = oauthSession.LongLivedToken,
                    IsActive = true,
                    TenantId = oauthSession.TenantId
                });
            }

            linked++;
        }

        await _adAccountRepo.SaveChangesAsync();
        return Ok(new { linked, projectId = oauthSession.ProjectId });
    }

    private string CallbackUri => $"{_config["AppUrl"] ?? "https://studiomeshcrm.com"}/api/facebook/oauth/callback";
    private string FrontendUrl => _config["AppUrl"] ?? "https://studiomeshcrm.com";
}

// ── Supporting types ────────────────────────────────────────────────────────

internal record FacebookOAuthState
{
    public Guid ProjectId { get; init; }
    public Guid TenantId { get; init; }
}

internal record FacebookOAuthSession
{
    public Guid ProjectId { get; init; }
    public Guid TenantId { get; init; }
    public string LongLivedToken { get; init; } = string.Empty;
    public List<FacebookAdAccount> AdAccounts { get; init; } = new();
}

internal record FacebookTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; init; }
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
