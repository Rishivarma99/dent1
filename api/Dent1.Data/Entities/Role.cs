namespace Dent1.Data.Entities;

/// <summary>
/// Global role template (not tenant-specific).
/// All tenants share the same role definitions.
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
