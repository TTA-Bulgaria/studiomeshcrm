using Crm.Application.Exceptions;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crm.Infrastructure.Services;

public class MetaAdsClient : IAdPlatformClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MetaAdsClient> _logger;

    public MetaAdsClient(HttpClient httpClient, ILogger<MetaAdsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public AdPlatform Platform => AdPlatform.Meta;

    public async Task<IEnumerable<AdMetric>> FetchDailyMetricsAsync(string externalAccountId, DateTime date, string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogWarning("FetchDailyMetrics skipped for {AccountId}: no access token", externalAccountId);
            return Enumerable.Empty<AdMetric>();
        }

        var dateString = date.ToString("yyyy-MM-dd");
        var url = $"https://graph.facebook.com/v19.0/{externalAccountId}/insights" +
                  $"?fields=spend,impressions,clicks,actions" +
                  $"&time_range={{\"since\":\"{dateString}\",\"until\":\"{dateString}\"}}" +
                  $"&access_token={accessToken}";

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await _httpClient.GetAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP error fetching Meta insights for {AccountId} on {Date}", externalAccountId, date);
            throw new AdPlatformException($"Network error fetching Meta insights: {ex.Message}");
        }

        var body = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            // Facebook error codes: 190 = invalid/expired token, 102 = session key invalid
            var isTokenError = body.Contains("\"code\":190") || body.Contains("\"code\":102")
                               || (int)httpResponse.StatusCode == 401;

            _logger.LogWarning(
                "Meta API returned {StatusCode} for {AccountId} on {Date}. TokenExpired={IsTokenError}. Body: {Body}",
                (int)httpResponse.StatusCode, externalAccountId, date, isTokenError, body);

            throw new AdPlatformException(
                $"Meta API error {(int)httpResponse.StatusCode} for account {externalAccountId}",
                isTokenExpired: isTokenError);
        }

        try
        {
            var response = JsonSerializer.Deserialize<MetaInsightsResponse>(body);

            if (response?.Data == null || response.Data.Count == 0)
            {
                _logger.LogInformation("No Meta insights data for {AccountId} on {Date} (no active campaigns)", externalAccountId, date);
                return Enumerable.Empty<AdMetric>();
            }

            return response.Data.Select(row => new AdMetric
            {
                Id = Guid.NewGuid(),
                Platform = AdPlatform.Meta,
                Spend = decimal.TryParse(row.Spend, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var s) ? s : 0,
                Impressions = long.TryParse(row.Impressions, out var imp) ? imp : 0,
                Clicks = long.TryParse(row.Clicks, out var cl) ? cl : 0,
                Conversions = row.Actions?.Sum(a => long.TryParse(a.Value, out var v) ? v : 0) ?? 0,
                Date = date
            });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Meta insights response for {AccountId}. Body: {Body}", externalAccountId, body);
            throw new AdPlatformException($"Failed to parse Meta insights response: {ex.Message}");
        }
    }
}

internal class MetaInsightsResponse
{
    [JsonPropertyName("data")]
    public List<MetaInsightsRow> Data { get; set; } = new();
}

internal class MetaInsightsRow
{
    [JsonPropertyName("spend")]
    public string? Spend { get; set; }

    [JsonPropertyName("impressions")]
    public string? Impressions { get; set; }

    [JsonPropertyName("clicks")]
    public string? Clicks { get; set; }

    [JsonPropertyName("actions")]
    public List<MetaAction>? Actions { get; set; }
}

internal class MetaAction
{
    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
