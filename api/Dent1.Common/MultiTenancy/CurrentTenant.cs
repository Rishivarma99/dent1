namespace Dent1.Common.MultiTenancy;

/// <summary>
/// Current tenant context implementation.
/// Scoped lifetime - one per HTTP request.
/// Tenant is resolved once in middleware and stored here.
/// </summary>
public sealed class CurrentTenant : ICurrentTenant
{
    public Guid TenantId { get; private set; }
    public bool IsResolved { get; private set; }

    /// <summary>
    /// Sets the tenant ID once. Cannot be changed after set.
    /// Throws InvalidOperationException if already resolved.
    /// </summary>
    public void SetTenant(Guid tenantId)
    {
        if (IsResolved)
            throw new InvalidOperationException("Tenant already resolved for this request.");

        TenantId = tenantId;
        IsResolved = true;
    }
}
