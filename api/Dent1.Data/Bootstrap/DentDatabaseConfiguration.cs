using Microsoft.EntityFrameworkCore;

namespace Dent1.Data;

public enum DentDatabaseProvider
{
    PostgreSql,
    SqlServer
}

public static class DentDatabaseConfiguration
{
    public static DentDatabaseProvider ResolveProvider(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection is missing.");
        }

        if (HasKey(connectionString, "Host") || HasKey(connectionString, "Username"))
        {
            return DentDatabaseProvider.PostgreSql;
        }

        if (HasKey(connectionString, "Data Source") ||
            HasKey(connectionString, "Server") ||
            HasKey(connectionString, "Initial Catalog") ||
            HasKey(connectionString, "Integrated Security") ||
            HasKey(connectionString, "Trusted_Connection"))
        {
            return DentDatabaseProvider.SqlServer;
        }

        throw new InvalidOperationException(
            "DefaultConnection must be a PostgreSQL or SQL Server connection string.");
    }

    public static DbContextOptionsBuilder UseConfiguredDatabase(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString)
    {
        return ResolveProvider(connectionString) switch
        {
            DentDatabaseProvider.PostgreSql => optionsBuilder.UseNpgsql(
                connectionString,
                npgsql => npgsql.EnableRetryOnFailure()),

            DentDatabaseProvider.SqlServer => optionsBuilder.UseSqlServer(
                connectionString,
                sqlServer => sqlServer.EnableRetryOnFailure()),

            _ => throw new InvalidOperationException("Unsupported database provider.")
        };
    }

    private static bool HasKey(string connectionString, string key)
    {
        return connectionString.Contains($"{key}=", StringComparison.OrdinalIgnoreCase);
    }
}
