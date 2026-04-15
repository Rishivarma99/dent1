using Dent1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface IPatientSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class PatientSeedService : IPatientSeedService
{
    private readonly DentContext _dbContext;

    public PatientSeedService(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var patients = new[]
        {
            new Patient { Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"), TenantId = SeedIds.DefaultTenantId, Name = "Rishi Alluri", Phone = "9876543210", CreatedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0002-0000-0000-000000000002"), TenantId = SeedIds.DefaultTenantId, Name = "Priya Sharma", Phone = "9876543210", CreatedAt = new DateTime(2026, 1, 20, 11, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0003-0000-0000-000000000003"), TenantId = SeedIds.DefaultTenantId, Name = "Amit Patel", Phone = "9123456789", CreatedAt = new DateTime(2026, 2, 1, 9, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0004-0000-0000-000000000004"), TenantId = SeedIds.DefaultTenantId, Name = "Sneha Reddy", Phone = "9988776655", CreatedAt = new DateTime(2026, 2, 5, 14, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0005-0000-0000-000000000005"), TenantId = SeedIds.DefaultTenantId, Name = "Vikram Singh", Phone = "9123456789", CreatedAt = new DateTime(2026, 2, 10, 8, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0006-0000-0000-000000000006"), TenantId = SeedIds.DefaultTenantId, Name = "Ananya Gupta", Phone = "9112233445", CreatedAt = new DateTime(2026, 2, 15, 16, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0007-0000-0000-000000000007"), TenantId = SeedIds.DefaultTenantId, Name = "Rajesh Kumar", Phone = "9876543210", CreatedAt = new DateTime(2026, 2, 20, 10, 30, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0008-0000-0000-000000000008"), TenantId = SeedIds.DefaultTenantId, Name = "Meena Iyer", Phone = "9556677889", CreatedAt = new DateTime(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0009-0000-0000-000000000009"), TenantId = SeedIds.DefaultTenantId, Name = "Suresh Nair", Phone = "9445566778", CreatedAt = new DateTime(2026, 3, 10, 15, 0, 0, DateTimeKind.Utc) },
            new Patient { Id = Guid.Parse("a1b2c3d4-0010-0000-0000-000000000010"), TenantId = SeedIds.DefaultTenantId, Name = "Deepa Menon", Phone = "9334455667", CreatedAt = new DateTime(2026, 3, 15, 9, 30, 0, DateTimeKind.Utc) }
        };

        foreach (var seed in patients)
        {
            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == seed.Id, cancellationToken);
            if (patient is null)
            {
                _dbContext.Patients.Add(seed);
                continue;
            }

            patient.TenantId = seed.TenantId;
            patient.Name = seed.Name;
            patient.Phone = seed.Phone;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
