namespace Dent1.Common.MultiTenancy;

/// <summary>
/// Represents the current tenant context for the request.
/// Scoped - one per request.
/// </summary>
public interface ICurrentTenant
{
    Guid TenantId { get; }
    bool IsResolved { get; }
}
