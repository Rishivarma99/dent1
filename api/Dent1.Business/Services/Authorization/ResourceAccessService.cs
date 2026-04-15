using Dent1.Common.Authorization;

namespace Dent1.Business.Security;

/// <summary>
/// Checks if user can access a specific resource based on scope.
/// Scope is the second authorization check, applied after permission is granted.
/// </summary>
public interface IResourceAccessService
{
    /// <summary>
    /// Check if user can access the specified resource within scope.
    /// </summary>
    /// <remarks>
    /// Must be called immediately after loading target resource to verify
    /// user has access (e.g., assigned doctor, own branch, etc.).
    /// </remarks>
    bool CanAccess(UserContext user, string permissionCode, Guid resourceTenantId, Guid? resourceOwnerId = null);

    /// <summary>
    /// Check if user can access resource in their assigned scope.
    /// </summary>
    bool CanAccessAssigned(UserContext user, Guid resourceOwnerId);

    /// <summary>
    /// Check if user can access resource in their own scope.
    /// </summary>
    bool CanAccessOwn(UserContext user, Guid resourceOwnerId);
}

public sealed class ResourceAccessService : IResourceAccessService
{
    public bool CanAccess(UserContext user, string permissionCode, Guid resourceTenantId, Guid? resourceOwnerId = null)
    {
        if (user == null)
            return false;

        // Tenant boundary is mandatory
        if (user.TenantId != resourceTenantId)
            return false;

        var scopes = user.GetScopes(permissionCode);

        // If no scopes defined, default to tenant scope
        if (scopes.Length == 0)
            return true;

        // Check if user has necessary scope type
        foreach (var scope in scopes)
        {
            if (scope == ScopeType.All)
                return true;

            if (scope == ScopeType.Tenant)
                return true; // Already checked tenant boundary above

            if ((scope == ScopeType.Own || scope == ScopeType.Assigned) && resourceOwnerId.HasValue)
            {
                if (CanAccessOwn(user, resourceOwnerId.Value))
                    return true;
            }
        }

        return false;
    }

    public bool CanAccessAssigned(UserContext user, Guid resourceOwnerId)
    {
        if (user == null)
            return false;

        return user.UserId == resourceOwnerId;
    }

    public bool CanAccessOwn(UserContext user, Guid resourceOwnerId)
    {
        if (user == null)
            return false;

        return user.UserId == resourceOwnerId;
    }
}
