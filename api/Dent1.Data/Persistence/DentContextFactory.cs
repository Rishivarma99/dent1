using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Dent1.Data;

public class DentContextFactory : IDesignTimeDbContextFactory<DentContext>
{
    public DentContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Dent1.Api"))
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<DentContext>();
        optionsBuilder.UseConfiguredDatabase(connectionString);

        return new DentContext(optionsBuilder.Options);
    }
}
