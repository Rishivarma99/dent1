using Dent1.Data.Enums;

namespace Dent1.Data.Entities;

/// <summary>
/// Tenant-specific role permission override.
/// Allows a specific tenant to customize permissions for a role.
/// For example, Tenant A can give Assistant the "appointment.reschedule" permission
/// while Tenant B does not have this for their Assistants.
/// </summary>
public class TenantRolePermissionOverride
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public PermissionOverrideEffect Effect { get; set; }
    public DateTime CreatedAt { get; set; }
}
