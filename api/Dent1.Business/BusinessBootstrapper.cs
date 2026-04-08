using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Dent1.Business.Behaviors;
using Dent1.Business.Cqrs;
using Dent1.Business.Commands;
using Dent1.Business.DTOs;
using Dent1.Business.Queries;
using Dent1.Business.Security;
using Dent1.Business.Services;
using Dent1.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Dent1.Business;

public static class BusinessBootstrapper
{
    public static void Register(IServiceCollection services)
    {
        // ── MediatR + Pipeline Behaviors ──────────────────────────────────────
        // Behaviors are executed in registration order (outermost → innermost):
        //   LoggingBehavior → ValidationBehavior → TransactionBehavior → Handler
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(BusinessBootstrapper).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        // Scans this assembly and registers all AbstractValidator<T> classes with DI.
        // ValidationBehavior picks them up via IEnumerable<IValidator<TRequest>>.
        services.AddValidatorsFromAssembly(typeof(BusinessBootstrapper).Assembly);

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IPatientService, PatientService>();

        // Explicit CQRS registrations for Doctors module only (no MediatR).
        services.AddScoped<ICommandHandler<CreateDoctorCommand, Guid>, CreateDoctorCommandHandler>();
        services.AddScoped<IQueryHandler<GetAllDoctorsQuery, List<DoctorDto>>, GetAllDoctorsQueryHandler>();
        services.AddScoped<IQueryHandler<GetDoctorByIdQuery, DoctorDto?>, GetDoctorByIdQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateDoctorCommand, bool>, UpdateDoctorCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteDoctorCommand, bool>, DeleteDoctorCommandHandler>();
    }
}
