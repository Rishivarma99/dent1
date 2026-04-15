using Dent1.Common.MultiTenancy;
using Dent1.Data.Interfaces;

namespace Dent1.Business.MultiTenancy;

/// <summary>
/// Resolves tenant from host by looking up in tenant repository.
/// For now, simple implementation - in production may handle subdomains, custom domains, etc.
/// </summary>
public sealed class TenantHostResolver : ITenantHostResolver
{
    private readonly ITenantRepository _tenantRepository;

    public TenantHostResolver(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Guid?> ResolveTenantIdByHostAsync(string host, CancellationToken cancellationToken)
    {
        // For now, we don't have host-to-tenant mapping.
        // Return null and let JWT claim be the primary source.
        // TODO: Implement host-based tenant resolution (subdomains, custom domains, etc.)
        await Task.CompletedTask;
        return null;
    }
}
