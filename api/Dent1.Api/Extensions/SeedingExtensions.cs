using Dent1.Data.Repositories.Seeding;
using Dent1.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Api.Extensions;

// ONLY CALLED IN DEVELOPMENT
public static class SeedingExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DentContext>();
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        var seedService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedService>();
        await seedService.SeedAsync(cancellationToken);
    }
}