using Microsoft.Extensions.DependencyInjection;
using Dent1.Business.Security;
using Dent1.Business.Services;
using Dent1.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dent1.Business;

public static class BusinessBootstrapper
{
    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IPatientService, PatientService>();
    }
}
