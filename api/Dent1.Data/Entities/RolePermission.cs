namespace Dent1.Data.Entities;

/// <summary>
/// Global default permission set for each role.
/// Defines which permissions belong to each role by default.
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
