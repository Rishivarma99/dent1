using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dent1.Data;

public class DentContextFactory : IDesignTimeDbContextFactory<DentContext>
{
    public DentContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DENT1_CONNECTION")
            ?? "Host=localhost;Database=Dent1;Username=postgres;Password=root";

        var optionsBuilder = new DbContextOptionsBuilder<DentContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DentContext(optionsBuilder.Options);
    }
}
