using Dent1.Common.Authorization;

namespace Dent1.Business.Security;

/// <summary>
/// Current user context resolved from JWT and database.
/// Carries user identity and effective permissions/scopes for the request.
/// </summary>
public record UserContext
{
    public required Guid UserId { get; init; }
    public required Guid TenantId { get; init; }
    public required string Username { get; init; }
    public required string[] RoleNames { get; init; }
    public required Dictionary<string, ScopeType[]> PermissionScopes { get; init; }

    /// <summary>
    /// Check if user has a specific permission.
    /// </summary>
    public bool HasPermission(string permissionCode)
    {
        return PermissionScopes.ContainsKey(permissionCode);
    }

    /// <summary>
    /// Get scope types for a permission.
    /// </summary>
    public ScopeType[] GetScopes(string permissionCode)
    {
        return PermissionScopes.TryGetValue(permissionCode, out var scopes) 
            ? scopes 
            : Array.Empty<ScopeType>();
    }
}
