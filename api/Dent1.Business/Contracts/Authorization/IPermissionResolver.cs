namespace Dent1.Business.Abstractions;

/// <summary>
/// Resolves the final effective permissions for a user.
/// Computes: global role permissions + tenant overrides + user overrides.
/// </summary>
public interface IPermissionResolver
{
    Task<IReadOnlyCollection<string>> ResolveAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken);
}
