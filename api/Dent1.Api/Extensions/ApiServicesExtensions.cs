using Dent1.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dent1.Api.Extensions;

public static class ApiServicesExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserSeedService, UserSeedService>();

        return services;
    }
}