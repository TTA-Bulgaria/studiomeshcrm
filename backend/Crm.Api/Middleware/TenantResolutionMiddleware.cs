using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Api.Middleware;

public class TenantResolutionMiddleware
{
    private const string BaseDomain = "studiomeshcrm.com";
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var host = context.Request.Host.Host.ToLowerInvariant();

        // Only resolve a tenant from subdomains of our known base domain.
        // Rejects any Host header that isn't *.studiomeshcrm.com.
        if (!host.EndsWith("." + BaseDomain, StringComparison.Ordinal))
        {
            await _next(context);
            return;
        }

        var subdomain = host[..^(BaseDomain.Length + 1)]; // strip ".studiomeshcrm.com"

        // Ignore infrastructure subdomains — they don't map to tenants.
        if (subdomain is "app" or "www" or "staging" or "api")
        {
            await _next(context);
            return;
        }

        // Only allow slug-safe characters to reach the DB query.
        if (!System.Text.RegularExpressions.Regex.IsMatch(subdomain, @"^[a-z0-9-]+$"))
        {
            await _next(context);
            return;
        }

        var tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug == subdomain);

        if (tenant != null)
            context.Items["TenantId"] = tenant.Id;

        await _next(context);
    }
}
