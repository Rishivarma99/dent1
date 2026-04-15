namespace Dent1.Data.Entities;

/// <summary>
/// Global permission master (not tenant-specific).
/// All permissions are defined once at the platform level.
/// </summary>
public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
