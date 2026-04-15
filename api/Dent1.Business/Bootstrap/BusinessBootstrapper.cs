using Microsoft.Extensions.DependencyInjection;
using Dent1.Business.Security;
using Dent1.Business.Services;
using Dent1.Business.Abstractions;
using Dent1.Business.Dispatching;
using Dent1.Business.Pipeline;
using Dent1.Business.MultiTenancy;
using Dent1.Common.MultiTenancy;
using Dent1.Data.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace Dent1.Business;

public static class BusinessBootstrapper
{
    public static void Register(IServiceCollection services)
    {
        // Multi-tenancy - scoped lifetime, one per request
        services.AddScoped<CurrentTenant>();
        services.AddScoped<ICurrentTenant>(sp => sp.GetRequiredService<CurrentTenant>());
        services.AddScoped<ITenantHostResolver, TenantHostResolver>();

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<ITokenValidationService, TokenValidationService>();
        services.AddScoped<ITokenAuthStateReader, TokenAuthStateReader>();

        // Permission and authorization services
        services.AddScoped<IPermissionResolver, PermissionResolver>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IResourceAccessService, ResourceAccessService>();

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        var assembly = typeof(BusinessBootstrapper).Assembly;

        RegisterByOpenGeneric(services, assembly, typeof(ICommandHandler<,>));
        RegisterByOpenGeneric(services, assembly, typeof(IQueryHandler<,>));
        RegisterByOpenGeneric(services, assembly, typeof(IValidator<>));
    }

    private static void RegisterByOpenGeneric(IServiceCollection services, Assembly assembly, Type openGeneric)
    {
        var implementationTypes = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false });

        foreach (var implementationType in implementationTypes)
        {
            var serviceTypes = implementationType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric)
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType);
            }
        }
    }
}
