using Dent1.Business.MultiTenancy;
using Dent1.Business.Security;
using Dent1.Common.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dent1.Api.Extensions;

public static class ApiServicesExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUser, CurrentUserAccessor>();

        return services;
    }
}
