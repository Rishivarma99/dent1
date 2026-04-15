using Dent1.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface ITenantSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class TenantSeedService : ITenantSeedService
{
    private readonly DentContext _dbContext;

    public TenantSeedService(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == SeedIds.DefaultTenantId, cancellationToken);
        if (tenant is null)
        {
            _dbContext.Tenants.Add(new Tenant
            {
                Id = SeedIds.DefaultTenantId,
                Name = "Default Clinic",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
        else
        {
            tenant.Name = "Default Clinic";
            tenant.IsActive = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
