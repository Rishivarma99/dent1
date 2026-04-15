namespace Dent1.Business.Security;

/// <summary>
/// Checks if user has required permission.
/// Permission is the first authorization check.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if user has the required permission.
    /// </summary>
    bool HasPermission(UserContext user, string permissionCode);
}

public sealed class PermissionService : IPermissionService
{
    public bool HasPermission(UserContext user, string permissionCode)
    {
        if (user == null)
            return false;

        return user.HasPermission(permissionCode);
    }
}
