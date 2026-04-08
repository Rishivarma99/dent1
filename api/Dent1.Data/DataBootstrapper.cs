using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dent1.Data.Interfaces;
using Dent1.Data.Repositories;

namespace Dent1.Data;

public static class DataBootstrapper
{
    public static void Register(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DentContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
