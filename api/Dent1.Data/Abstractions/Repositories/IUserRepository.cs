using Dent1.Data.Entities;
using Dent1.Data.Enums;

namespace Dent1.Data.Interfaces;

public interface IUserRepository
{
    Task<Guid> AddAsync(string name, string email, string username, string phoneNumber, string passwordHash, UserRole role, Guid tenantId, bool isActive, CancellationToken cancellationToken);
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Get user by ID without tenant filter (used internally).
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get user by ID with tenant filter (secure - tenant-aware).
    /// </summary>
    Task<User?> GetByIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
    
    Task<bool> UpdateAsync(Guid id, string name, string email, string username, string phoneNumber, UserRole role, Guid tenantId, bool isActive, CancellationToken cancellationToken);
    
    /// <summary>
    /// Delete user with tenant verification.
    /// </summary>
    Task<bool> DeleteAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken);
    
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByUsernameAsync(string username, Guid? excludingUserId, CancellationToken cancellationToken);
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, Guid? excludingUserId, CancellationToken cancellationToken);
    Task<User?> GetByUsernameOrPhoneAsync(string usernameOrPhone, CancellationToken cancellationToken);
    Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken);
}
