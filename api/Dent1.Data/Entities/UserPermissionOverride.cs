using Dent1.Data.Enums;

namespace Dent1.Data.Entities;

/// <summary>
/// User-specific permission override.
/// Allows fine-grained permission customization for individual users.
/// For example, one main assistant gets "appointment.reschedule"
/// even though other assistants do not.
/// </summary>
public class UserPermissionOverride
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public PermissionOverrideEffect Effect { get; set; }
    public DateTime CreatedAt { get; set; }
}
