using Crm.Application.DTOs.AdMetrics;
using Crm.Application.Exceptions;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Application.Services;

public class AdMetricService : IAdMetricService
{
    private readonly IGenericRepository<AdMetric> _repository;
    private readonly IGenericRepository<Project> _projectRepository;
    private readonly IGenericRepository<Contract> _contractRepository;
    private readonly IGenericRepository<ProjectAdAccount> _adAccountRepository;
    private readonly IEnumerable<IAdPlatformClient> _platformClients;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ILogger<AdMetricService> _logger;

    public AdMetricService(
        IGenericRepository<AdMetric> repository,
        IGenericRepository<Project> projectRepository,
        IGenericRepository<Contract> contractRepository,
        IGenericRepository<ProjectAdAccount> adAccountRepository,
        IEnumerable<IAdPlatformClient> platformClients,
        ICurrentUserContext currentUserContext,
        ILogger<AdMetricService> logger)
    {
        _repository = repository;
        _projectRepository = projectRepository;
        _contractRepository = contractRepository;
        _adAccountRepository = adAccountRepository;
        _platformClients = platformClients;
        _currentUserContext = currentUserContext;
        _logger = logger;
    }

    private Guid TenantId => _currentUserContext.TenantId ?? Guid.Empty;

    public async Task<IEnumerable<AdMetricResponse>> GetProjectMetricsAsync(Guid projectId)
    {
        var metrics = await _repository.AsQueryable()
            .Where(m => m.ProjectId == projectId && m.TenantId == TenantId)
            .ToListAsync();
        return metrics.Select(MapToResponse);
    }

    public async Task<IEnumerable<AdMetricResponse>> GetAllAsync()
    {
        var metrics = await _repository.AsQueryable()
            .Where(m => m.TenantId == TenantId)
            .ToListAsync();
        return metrics.Select(MapToResponse);
    }

    public async Task<AdMetricAnalyticsResponse> GetProjectAnalyticsAsync(Guid projectId)
    {
        var metrics = await _repository.AsQueryable()
            .Where(m => m.ProjectId == projectId && m.TenantId == TenantId)
            .ToListAsync();

        var contracts = await _contractRepository.AsQueryable()
            .Where(c => c.ProjectId == projectId && c.TenantId == TenantId && c.Status == ContractStatus.Signed)
            .ToListAsync();

        var totalSpend = metrics.Sum(m => m.Spend);
        var totalRevenue = contracts.Sum(c => c.TotalAmount);

        return new AdMetricAnalyticsResponse
        {
            ProjectId = projectId,
            TotalSpend = totalSpend,
            TotalImpressions = metrics.Sum(m => m.Impressions),
            TotalClicks = metrics.Sum(m => m.Clicks),
            TotalConversions = metrics.Sum(m => m.Conversions),
            RawMetrics = metrics.Select(MapToResponse).ToList(),
            ROAS = totalSpend > 0 ? totalRevenue / totalSpend : 0,
            ProjectROI = totalSpend > 0 ? (totalRevenue - totalSpend) / totalSpend * 100 : 0
        };
    }

    public async Task<AdMetricAnalyticsResponse> GetGlobalAnalyticsAsync()
    {
        var metrics = await _repository.AsQueryable()
            .Where(m => m.TenantId == TenantId)
            .ToListAsync();

        var contracts = await _contractRepository.AsQueryable()
            .Where(c => c.TenantId == TenantId && c.Status == ContractStatus.Signed)
            .ToListAsync();

        var totalSpend = metrics.Sum(m => m.Spend);
        var totalRevenue = contracts.Sum(c => c.TotalAmount);

        return new AdMetricAnalyticsResponse
        {
            ProjectId = Guid.Empty,
            TotalSpend = totalSpend,
            TotalImpressions = metrics.Sum(m => m.Impressions),
            TotalClicks = metrics.Sum(m => m.Clicks),
            TotalConversions = metrics.Sum(m => m.Conversions),
            RawMetrics = metrics.Select(MapToResponse).ToList(),
            ROAS = totalSpend > 0 ? totalRevenue / totalSpend : 0,
            ProjectROI = totalSpend > 0 ? (totalRevenue - totalSpend) / totalSpend * 100 : 0
        };
    }

    public async Task SyncMetricsAsync(Guid projectId)
    {
        var accounts = await _adAccountRepository.AsQueryable()
            .Where(a => a.ProjectId == projectId && a.TenantId == TenantId && a.IsActive)
            .ToListAsync();

        foreach (var account in accounts)
        {
            // Token expired — mark inactive so the user knows to reconnect
            if (account.TokenExpiresAt.HasValue && account.TokenExpiresAt.Value <= DateTime.UtcNow)
            {
                _logger.LogWarning(
                    "Access token expired for ad account {AccountId} (project {ProjectId}). Marking inactive.",
                    account.Id, projectId);
                account.IsActive = false;
                await _adAccountRepository.UpdateAsync(account);
                continue;
            }

            var client = _platformClients.FirstOrDefault(c => c.Platform == account.Platform);
            if (client == null) continue;

            for (int i = 0; i <= 1; i++)
            {
                var date = DateTime.UtcNow.AddDays(-i).Date;

                IEnumerable<AdMetric> newMetrics;
                try
                {
                    newMetrics = await client.FetchDailyMetricsAsync(
                        account.ExternalAccountId, date, account.AccessToken ?? string.Empty);
                }
                catch (AdPlatformException ex) when (ex.IsTokenExpired)
                {
                    _logger.LogWarning(
                        "Token rejected by platform for ad account {AccountId}. Marking inactive.",
                        account.Id);
                    account.IsActive = false;
                    await _adAccountRepository.UpdateAsync(account);
                    break; // Stop syncing this account's dates
                }
                catch (AdPlatformException ex)
                {
                    _logger.LogError(
                        "Platform error syncing ad account {AccountId} on {Date}: {Message}",
                        account.Id, date, ex.Message);
                    continue; // Skip this date, try next
                }

                foreach (var m in newMetrics)
                {
                    var existing = await _repository.AsQueryable()
                        .FirstOrDefaultAsync(em => em.AdAccountId == account.Id && em.Date.Date == date.Date);

                    if (existing != null)
                    {
                        existing.Spend = m.Spend;
                        existing.Impressions = m.Impressions;
                        existing.Clicks = m.Clicks;
                        existing.Conversions = m.Conversions;
                        await _repository.UpdateAsync(existing);
                    }
                    else
                    {
                        m.ProjectId = projectId;
                        m.AdAccountId = account.Id;
                        m.TenantId = account.TenantId;
                        await _repository.AddAsync(m);
                    }
                }
            }
        }
        await _repository.SaveChangesAsync();
    }

    public async Task<decimal> GetSpendByRangeAsync(Guid projectId, DateTime start, DateTime end)
    {
        var metrics = await _repository.AsQueryable()
            .Where(m => m.ProjectId == projectId && m.TenantId == TenantId && m.Date >= start && m.Date <= end)
            .ToListAsync();

        return metrics.Sum(m => m.Spend);
    }

    public async Task<AdMetricResponse> CreateAsync(CreateAdMetricRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null || project.TenantId != TenantId)
            throw new UnauthorizedAccessException("Project not found or access denied.");

        var metric = new AdMetric
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            Platform = request.Platform,
            Spend = request.Spend,
            Impressions = request.Impressions,
            Clicks = request.Clicks,
            Conversions = request.Conversions,
            Date = request.Date,
            TenantId = TenantId
        };

        await _repository.AddAsync(metric);
        await _repository.SaveChangesAsync();

        return MapToResponse(metric);
    }

    private AdMetricResponse MapToResponse(AdMetric m) => new()
    {
        Id = m.Id,
        ProjectId = m.ProjectId,
        Platform = m.Platform,
        Spend = m.Spend,
        Impressions = m.Impressions,
        Clicks = m.Clicks,
        Conversions = m.Conversions,
        Date = m.Date,
        CreatedAt = m.CreatedAt
    };
}
