using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;

namespace Crm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/adaccounts")]
public class ProjectAdAccountsController : ControllerBase
{
    private readonly IGenericRepository<ProjectAdAccount> _repository;
    private readonly IGenericRepository<Project> _projectRepository;
    private readonly IAdMetricService _adMetricService;
    private readonly ICurrentUserContext _userContext;

    public ProjectAdAccountsController(
        IGenericRepository<ProjectAdAccount> repository,
        IGenericRepository<Project> projectRepository,
        IAdMetricService adMetricService,
        ICurrentUserContext userContext)
    {
        _repository = repository;
        _projectRepository = projectRepository;
        _adMetricService = adMetricService;
        _userContext = userContext;
    }

    private async Task<bool> ProjectBelongsToTenantAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);
        return project != null && project.TenantId == _userContext.TenantId;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectAdAccount>>> GetAll(Guid projectId)
    {
        if (!await ProjectBelongsToTenantAsync(projectId))
            return Forbid();

        var accounts = await _repository.AsQueryable()
            .Where(a => a.ProjectId == projectId)
            .ToListAsync();
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectAdAccount>> Create(Guid projectId, ProjectAdAccount account)
    {
        if (!await ProjectBelongsToTenantAsync(projectId))
            return Forbid();

        account.ProjectId = projectId;
        account.Id = Guid.NewGuid();

        await _repository.AddAsync(account);
        await _repository.SaveChangesAsync();

        await _adMetricService.SyncMetricsAsync(projectId);

        return Ok(account);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id)
    {
        if (!await ProjectBelongsToTenantAsync(projectId))
            return Forbid();

        var account = await _repository.GetByIdAsync(id);
        if (account == null) return NotFound();

        // Verify the account belongs to this project (not just any project in the tenant)
        if (account.ProjectId != projectId)
            return Forbid();

        _repository.Delete(account);
        await _repository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync(Guid projectId)
    {
        if (!await ProjectBelongsToTenantAsync(projectId))
            return Forbid();

        await _adMetricService.SyncMetricsAsync(projectId);
        return Ok(new { message = "Sync triggered successfully" });
    }
}
