using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Dent1.Data.Interfaces;
using Dent1.Data.Repositories;
using Dent1.Data.Repositories.Seeding;

namespace Dent1.Data;

public static class DataBootstrapper
{
    public static void Register(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DentContext>(options =>
            options
                .UseSqlServer(connectionString)
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // SEEDING SERVICES
        services.AddScoped<ITenantSeedService, TenantSeedService>();
        services.AddScoped<IRoleSeedService, RoleSeedService>();
        services.AddScoped<IPermissionSeedService, PermissionSeedService>();
        services.AddScoped<IRolePermissionSeedService, RolePermissionSeedService>();
        services.AddScoped<IUserSeedService, UserSeedService>();
        services.AddScoped<IUserPermissionOverrideSeedService, UserPermissionOverrideSeedService>();
        services.AddScoped<IPatientSeedService, PatientSeedService>();
        services.AddScoped<IDatabaseSeedService, DatabaseSeedService>();
    }
}
