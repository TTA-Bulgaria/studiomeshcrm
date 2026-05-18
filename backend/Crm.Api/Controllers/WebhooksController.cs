using System.Security.Cryptography;
using System.Text;
using Crm.Application.DTOs.AdMetrics;
using Crm.Application.DTOs.Leads;
using Crm.Application.Services;
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

public class MetaWebhookRequest
{
    public string? lead_id { get; set; }
    public string? form_id { get; set; }
    public Dictionary<string, string>? field_data { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly LeadService _leadService;
    private readonly AdMetricService _adMetricService;
    private readonly ILogger<WebhooksController> _logger;
    private readonly IConfiguration _config;

    public WebhooksController(
        LeadService leadService,
        AdMetricService adMetricService,
        ILogger<WebhooksController> logger,
        IConfiguration config)
    {
        _leadService = leadService;
        _adMetricService = adMetricService;
        _logger = logger;
        _config = config;
    }

    [HttpPost("meta/lead")]
    public async Task<IActionResult> MetaLeadWebhook()
    {
        if (!await VerifyMetaSignatureAsync())
            return Unauthorized(new { error = "Invalid signature" });

        MetaWebhookRequest? payload;
        try
        {
            payload = await System.Text.Json.JsonSerializer.DeserializeAsync<MetaWebhookRequest>(Request.Body);
        }
        catch
        {
            return BadRequest(new { error = "Invalid payload" });
        }

        _logger.LogInformation("Received verified Meta Lead Webhook");

        var request = new CreateLeadRequest
        {
            Title = $"Meta Lead: {payload?.lead_id ?? "Unknown"}",
            Description = $"Ingested from Meta at {DateTime.UtcNow}."
        };

        await _leadService.CreateAsync(request);
        return Ok(new { status = "success" });
    }

    [HttpGet("meta/lead")]
    public IActionResult MetaLeadWebhookVerification([FromQuery(Name = "hub.mode")] string? mode,
        [FromQuery(Name = "hub.verify_token")] string? verifyToken,
        [FromQuery(Name = "hub.challenge")] string? challenge)
    {
        var expectedToken = _config["MetaAds:WebhookVerifyToken"];
        if (mode == "subscribe" && verifyToken == expectedToken && !string.IsNullOrEmpty(challenge))
            return Ok(int.Parse(challenge));
        return Unauthorized();
    }

    [HttpPost("google/performance")]
    public async Task<IActionResult> GooglePerformanceWebhook([FromBody] CreateAdMetricRequest request)
    {
        if (!VerifySharedSecret("Google:WebhookSecret"))
            return Unauthorized(new { error = "Invalid token" });

        _logger.LogInformation("Received Google Performance Webhook for project {ProjectId}", request.ProjectId);
        await _adMetricService.CreateAsync(request);
        return Ok(new { status = "success" });
    }

    [HttpPost("tiktok/performance")]
    public async Task<IActionResult> TikTokPerformanceWebhook([FromBody] CreateAdMetricRequest request)
    {
        if (!VerifySharedSecret("TikTok:WebhookSecret"))
            return Unauthorized(new { error = "Invalid token" });

        _logger.LogInformation("Received TikTok Performance Webhook for project {ProjectId}", request.ProjectId);
        await _adMetricService.CreateAsync(request);
        return Ok(new { status = "success" });
    }

    // Reads the raw body and compares against Meta's X-Hub-Signature-256 header.
    // Must be called before any model binding consumes the body stream.
    private async Task<bool> VerifyMetaSignatureAsync()
    {
        var appSecret = _config["MetaAds:AppSecret"];
        if (string.IsNullOrEmpty(appSecret))
        {
            _logger.LogWarning("MetaAds:AppSecret is not configured — rejecting webhook");
            return false;
        }

        if (!Request.Headers.TryGetValue("X-Hub-Signature-256", out var signatureHeader))
            return false;

        var signature = signatureHeader.ToString();
        if (!signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
            return false;

        Request.EnableBuffering();
        var body = await new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
        Request.Body.Position = 0;

        var keyBytes = Encoding.UTF8.GetBytes(appSecret);
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var computedHash = HMACSHA256.HashData(keyBytes, bodyBytes);
        var computedSignature = "sha256=" + Convert.ToHexString(computedHash).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(signature.ToLowerInvariant()));
    }

    // Google and TikTok webhooks use a shared bearer secret in the Authorization header.
    private bool VerifySharedSecret(string configKey)
    {
        var secret = _config[configKey];
        if (string.IsNullOrEmpty(secret)) return false;

        var authHeader = Request.Headers.Authorization.ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return false;

        var provided = Encoding.UTF8.GetBytes(authHeader["Bearer ".Length..].Trim());
        var expected = Encoding.UTF8.GetBytes(secret);
        return CryptographicOperations.FixedTimeEquals(provided, expected);
    }
}
