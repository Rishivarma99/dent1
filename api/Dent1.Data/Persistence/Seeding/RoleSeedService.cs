using Dent1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface IRoleSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class RoleSeedService : IRoleSeedService
{
    private readonly DentContext _dbContext;

    public RoleSeedService(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var roles = new[]
        {
            new Role { Id = SeedIds.AdminRoleId, Name = "Admin", Description = "System administrator", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = SeedIds.DoctorRoleId, Name = "Doctor", Description = "Dental practitioner", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = SeedIds.ReceptionistRoleId, Name = "Receptionist", Description = "Front desk staff", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = SeedIds.AssistantRoleId, Name = "Assistant", Description = "Dental assistant", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = SeedIds.PatientRoleId, Name = "Patient", Description = "Patient user", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };

        foreach (var seed in roles)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == seed.Id, cancellationToken);
            if (role is null)
            {
                _dbContext.Roles.Add(seed);
                continue;
            }

            role.Name = seed.Name;
            role.Description = seed.Description;
            role.IsActive = seed.IsActive;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
