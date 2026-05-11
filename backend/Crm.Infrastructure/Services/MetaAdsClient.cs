using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Crm.Infrastructure.Services;

public class MetaAdsClient : IAdPlatformClient
{
    private readonly HttpClient _httpClient;

    public MetaAdsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public AdPlatform Platform => AdPlatform.Meta;

    public async Task<IEnumerable<AdMetric>> FetchDailyMetricsAsync(string externalAccountId, DateTime date, string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return Enumerable.Empty<AdMetric>();

        var dateString = date.ToString("yyyy-MM-dd");
        var url = $"https://graph.facebook.com/v19.0/{externalAccountId}/insights" +
                  $"?fields=spend,impressions,clicks,actions" +
                  $"&time_range={{\"since\":\"{dateString}\",\"until\":\"{dateString}\"}}" +
                  $"&access_token={accessToken}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<MetaInsightsResponse>(url);
            if (response?.Data == null || response.Data.Count == 0)
                return Enumerable.Empty<AdMetric>();

            return response.Data.Select(row => new AdMetric
            {
                Id = Guid.NewGuid(),
                Platform = AdPlatform.Meta,
                Spend = decimal.TryParse(row.Spend, out var s) ? s : 0,
                Impressions = long.TryParse(row.Impressions, out var imp) ? imp : 0,
                Clicks = long.TryParse(row.Clicks, out var cl) ? cl : 0,
                Conversions = row.Actions?.Sum(a => long.TryParse(a.Value, out var v) ? v : 0) ?? 0,
                Date = date
            });
        }
        catch
        {
            return Enumerable.Empty<AdMetric>();
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
