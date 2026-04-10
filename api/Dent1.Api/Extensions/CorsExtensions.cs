using Microsoft.Extensions.DependencyInjection;

namespace Dent1.Api.Extensions;

public static class CorsExtensions
{
    public const string FrontendCorsPolicyName = "FrontendCors";

    public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicyName, policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}