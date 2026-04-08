using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DentContext _context;

    public UserRepository(DentContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(string name, string email, string username, string phoneNumber, string passwordHash, UserRole role, bool isActive, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Username = username,
            PhoneNumber = phoneNumber,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = isActive,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user, cancellationToken);
        return user.Id;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string email, string username, string phoneNumber, UserRole role, bool isActive, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Name = name;
        user.Email = email;
        user.Username = username;
        user.PhoneNumber = phoneNumber;
        user.Role = role;
        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.IsDeleted = true;
        user.IsActive = false;
        user.RefreshTokenHash = null;
        user.RefreshTokenCreatedAt = null;
        user.RefreshTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public Task<bool> ExistsByUsernameAsync(string username, Guid? excludingUserId, CancellationToken cancellationToken)
    {
        return _context.Users.AnyAsync(u => u.Username == username && (!excludingUserId.HasValue || u.Id != excludingUserId.Value), cancellationToken);
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, Guid? excludingUserId, CancellationToken cancellationToken)
    {
        return _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && (!excludingUserId.HasValue || u.Id != excludingUserId.Value), cancellationToken);
    }

    public Task<User?> GetByUsernameOrPhoneAsync(string usernameOrPhone, CancellationToken cancellationToken)
    {
        return _context.Users.FirstOrDefaultAsync(
            u => u.Username == usernameOrPhone || u.PhoneNumber == usernameOrPhone,
            cancellationToken);
    }

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        return _context.Users.FirstOrDefaultAsync(
            u => u.RefreshTokenHash == refreshTokenHash,
            cancellationToken);
    }
}
