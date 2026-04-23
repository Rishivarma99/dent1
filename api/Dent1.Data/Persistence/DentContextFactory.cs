using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Dent1.Data;

public class DentContextFactory : IDesignTimeDbContextFactory<DentContext>
{
    public DentContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DENT1_CONNECTION")
            ?? "Data Source=DESKTOP-BDENLBT\\SQLEXPRESS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0";

        var optionsBuilder = new DbContextOptionsBuilder<DentContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new DentContext(optionsBuilder.Options);
    }
}
