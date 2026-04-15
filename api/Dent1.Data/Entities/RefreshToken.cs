namespace Dent1.Data.Entities;

/// <summary>
/// Refresh token for long-term authentication.
/// Refresh tokens are stored hashed in DB and rotated on use.
/// Supports multiple active tokens per user (multiple device login).
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public bool IsUsed { get; set; }
    public string? DeviceInfo { get; set; }
}
