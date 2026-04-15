using Dent1.Common.MultiTenancy;

namespace Dent1.Api.Middleware;

/// <summary>
/// Resolves tenant before any handler execution.
/// 
/// Tenant resolution order:
/// 1. Check JWT claim "tenant_id"
/// 2. Check host-based tenant
/// 3. Verify host and claim match (if both present)
/// 4. Set ICurrentTenant for the request
/// 
/// If both claim and host tenant exist and don't match, reject with 403.
/// If neither exists for a secured endpoint, the endpoint will fail with 401/403.
/// </summary>
public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(
        HttpContext context,
        CurrentTenant currentTenant,
        ITenantHostResolver tenantHostResolver)
    {
        try
        {
            // Try to resolve tenant from JWT claim
            var claimTenantValue = context.User.FindFirst("tenant_id")?.Value;
            Guid? claimTenantId = null;

            if (Guid.TryParse(claimTenantValue, out var parsedClaimTenantId))
                claimTenantId = parsedClaimTenantId;

            // Try to resolve tenant from host
            var host = context.Request.Host.Host;
            var hostTenantId = await tenantHostResolver.ResolveTenantIdByHostAsync(host, context.RequestAborted);

            // Verify host and claim match (if both present)
            if (claimTenantId.HasValue && hostTenantId.HasValue && claimTenantId.Value != hostTenantId.Value)
            {
                _logger.LogWarning(
                    "Tenant mismatch: claim={ClaimTenantId}, host={HostTenantId}",
                    claimTenantId.Value,
                    hostTenantId.Value);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Tenant mismatch.");
                return;
            }

            // Set current tenant (claim takes precedence over host)
            if (claimTenantId.HasValue)
            {
                currentTenant.SetTenant(claimTenantId.Value);
                _logger.LogInformation("Tenant resolved from JWT claim: {TenantId}", claimTenantId.Value);
            }
            else if (hostTenantId.HasValue)
            {
                currentTenant.SetTenant(hostTenantId.Value);
                _logger.LogInformation("Tenant resolved from host: {TenantId}", hostTenantId.Value);
            }
            else
            {
                _logger.LogInformation("No tenant resolved from claim or host");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in tenant resolution middleware");
            throw;
        }

        await _next(context);
    }
}
