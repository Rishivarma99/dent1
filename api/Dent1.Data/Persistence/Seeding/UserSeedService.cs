using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface IUserSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class UserSeedService : IUserSeedService
{
    private static readonly Dictionary<UserRole, Guid> RoleIds = new()
    {
        [UserRole.Admin] = SeedIds.AdminRoleId,
        [UserRole.Doctor] = SeedIds.DoctorRoleId,
        [UserRole.Receptionist] = SeedIds.ReceptionistRoleId,
        [UserRole.Assistant] = SeedIds.AssistantRoleId,
        [UserRole.Patient] = SeedIds.PatientRoleId
    };

    private readonly DentContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserSeedService(DentContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var users = new[]
        {
            new SeedUser(Guid.Parse("b1c2d3e4-1001-0000-0000-000000000001"), "Dr. Arjun Rao", "arjun.rao@dent1.local", "arjun.rao", "9000000001", UserRole.Doctor, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 1, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-1002-0000-0000-000000000002"), "Dr. Kavya Iyer", "kavya.iyer@dent1.local", "kavya.iyer", "9000000002", UserRole.Doctor, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 2, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-2001-0000-0000-000000000003"), "Patient Rohan", "rohan.patient@dent1.local", "rohan.patient", "9000000003", UserRole.Patient, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 3, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-2002-0000-0000-000000000004"), "Patient Meera", "meera.patient@dent1.local", "meera.patient", "9000000004", UserRole.Patient, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 4, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-3001-0000-0000-000000000005"), "Receptionist Nikhil", "nikhil.reception@dent1.local", "nikhil.reception", "9000000005", UserRole.Receptionist, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 5, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-3002-0000-0000-000000000006"), "Receptionist Sana", "sana.reception@dent1.local", "sana.reception", "9000000006", UserRole.Receptionist, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 6, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-4001-0000-0000-000000000007"), "Assistant Vivek", "vivek.assistant@dent1.local", "vivek.assistant", "9000000007", UserRole.Assistant, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 7, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-4002-0000-0000-000000000008"), "Assistant Neha", "neha.assistant@dent1.local", "neha.assistant", "9000000008", UserRole.Assistant, SeedIds.DefaultTenantId, false, new DateTime(2026, 3, 8, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-5001-0000-0000-000000000009"), "Admin Pranav", "pranav.admin@dent1.local", "pranav.admin", "9000000009", UserRole.Admin, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 9, 9, 0, 0, DateTimeKind.Utc)),
            new SeedUser(Guid.Parse("b1c2d3e4-5002-0000-0000-000000000010"), "Admin Anika", "anika.admin@dent1.local", "anika.admin", "9000000010", UserRole.Admin, SeedIds.DefaultTenantId, true, new DateTime(2026, 3, 10, 9, 0, 0, DateTimeKind.Utc))
        };

        const string defaultPassword = "Password@123";
        foreach (var seed in users)
        {
            var user = await _dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == seed.Id, cancellationToken);
            if (user is null)
            {
                user = new User
                {
                    Id = seed.Id,
                    CreatedAt = seed.CreatedAt
                };
                _dbContext.Users.Add(user);
            }

            user.Name = seed.Name;
            user.Email = seed.Email;
            user.Username = seed.Username;
            user.PhoneNumber = seed.PhoneNumber;
            user.Role = seed.Role;
            user.RoleId = RoleIds[seed.Role];
            user.TenantId = seed.TenantId;
            user.IsActive = seed.IsActive;
            user.IsDeleted = false;
            user.PasswordHash = _passwordHasher.HashPassword(user, defaultPassword);
            user.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record SeedUser(
        Guid Id, string Name, string Email, string Username, string PhoneNumber, UserRole Role, Guid TenantId, bool IsActive, DateTime CreatedAt);
}
