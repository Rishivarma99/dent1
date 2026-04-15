namespace Dent1.Common.MultiTenancy;

/// <summary>
/// Resolves tenant ID from host/subdomain.
/// Used in TenantResolutionMiddleware for host-based tenant resolution.
/// </summary>
public interface ITenantHostResolver
{
    /// <summary>
    /// Resolves tenant ID by hostname.
    /// Returns null if hostname doesn't match any tenant.
    /// </summary>
    Task<Guid?> ResolveTenantIdByHostAsync(string host, CancellationToken cancellationToken);
}
