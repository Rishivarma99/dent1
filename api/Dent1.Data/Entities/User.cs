using Dent1.Data.Enums;

namespace Dent1.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public string? RefreshTokenHash { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public DateTime? RefreshTokenCreatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
