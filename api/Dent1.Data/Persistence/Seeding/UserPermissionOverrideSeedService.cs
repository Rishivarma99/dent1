using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories.Seeding;

public interface IUserPermissionOverrideSeedService
{
    Task SeedAsync(CancellationToken cancellationToken);
}

public sealed class UserPermissionOverrideSeedService : IUserPermissionOverrideSeedService
{
    private readonly DentContext _dbContext;

    public UserPermissionOverrideSeedService(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        var id = Guid.Parse("30000000-0001-0000-0000-000000000001");
        var existing = await _dbContext.UserPermissionOverrides.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (existing is null)
        {
            _dbContext.UserPermissionOverrides.Add(new UserPermissionOverride
            {
                Id = id,
                UserId = Guid.Parse("b1c2d3e4-5001-0000-0000-000000000009"),
                PermissionId = SeedIds.PatientReadPermissionId,
                Effect = PermissionOverrideEffect.Deny,
                CreatedAt = new DateTime(2026, 3, 9, 9, 30, 0, DateTimeKind.Utc)
            });
        }
        else
        {
            existing.UserId = Guid.Parse("b1c2d3e4-5001-0000-0000-000000000009");
            existing.PermissionId = SeedIds.PatientReadPermissionId;
            existing.Effect = PermissionOverrideEffect.Deny;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
