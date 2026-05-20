using Dent1.Data.Repositories.Seeding;
using Dent1.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Dent1.Api.Extensions;

// ONLY CALLED IN DEVELOPMENT
public static class SeedingExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DentContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseMigration");

        if (string.Equals(
            dbContext.Database.ProviderName,
            "Microsoft.EntityFrameworkCore.SqlServer",
            StringComparison.Ordinal))
        {
            // SQL Server uses the current model because the existing migration is PostgreSQL-specific.
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
        else
        {
            await ApplyPendingMigrationsIfNeededAsync(dbContext, logger, cancellationToken);
        }

        var seedService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedService>();
        await seedService.SeedAsync(cancellationToken);
    }

    private static async Task ApplyPendingMigrationsIfNeededAsync(
        DentContext dbContext,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

        if (pendingMigrations.Count == 0)
        {
            logger.LogInformation("Database schema is up to date. Skipping migrations.");
            return;
        }

        // Tables exist but __EFMigrationsHistory is empty (e.g. prior EnsureCreated run).
        if (appliedMigrations.Count == 0 && await HasExistingSchemaAsync(dbContext, cancellationToken))
        {
            logger.LogWarning(
                "Existing schema detected without migration history. Recording {Count} baseline migration(s) without re-applying.",
                pendingMigrations.Count);
            await BaselinePendingMigrationsAsync(dbContext, pendingMigrations, cancellationToken);
            return;
        }

        logger.LogInformation(
            "Applying {Count} pending migration(s): {Migrations}",
            pendingMigrations.Count,
            string.Join(", ", pendingMigrations));
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    private static async Task<bool> HasExistingSchemaAsync(
        DentContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Database.CanConnectAsync(cancellationToken))
        {
            return false;
        }

        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != System.Data.ConnectionState.Open;
        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            // pg_class preserves quoted EF table names (e.g. "Patients").
            command.CommandText = """
                SELECT EXISTS (
                    SELECT 1
                    FROM pg_catalog.pg_class c
                    JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                    WHERE n.nspname = 'public'
                      AND c.relkind = 'r'
                      AND c.relname = 'Patients'
                );
                """;
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is true or 1 or (long)1;
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static async Task BaselinePendingMigrationsAsync(
        DentContext dbContext,
        IReadOnlyList<string> pendingMigrations,
        CancellationToken cancellationToken)
    {
        var productVersion = ProductInfo.GetVersion();

        await dbContext.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );
            """, cancellationToken);

        foreach (var migrationId in pendingMigrations)
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ({0}, {1})
                ON CONFLICT ("MigrationId") DO NOTHING;
                """,
                migrationId,
                productVersion,
                cancellationToken);
        }
    }
}
