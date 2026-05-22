using System.Text.RegularExpressions;
using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Data;
using Dent1.Business.Features.Doctors.Models;
using Dent1.Business.Features.Users.Commands.CreateUser;
using Dent1.Common.MultiTenancy;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Dent1.Business.Features.Doctors.Commands.CreateDoctor;

public sealed partial class CreateDoctorCommandHandler : ICommandHandler<CreateDoctorCommand, DoctorReadModel>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly IConfiguration _configuration;

    public CreateDoctorCommandHandler(
        ICommandDispatcher commandDispatcher,
        IUserRepository userRepository,
        ICurrentTenant currentTenant,
        IConfiguration configuration)
    {
        _commandDispatcher = commandDispatcher;
        _userRepository = userRepository;
        _currentTenant = currentTenant;
        _configuration = configuration;
    }

    public async Task<DoctorReadModel> Handle(CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
        {
            throw new InvalidOperationException("Tenant not resolved.");
        }

        var username = await ResolveUniqueUsernameAsync(ToUsernameSlug(command.Name), cancellationToken);
        var phone = await ResolveUniquePhoneAsync(cancellationToken);
        var email = $"{username}@dent1.local";
        var password = _configuration["Seeding:DefaultPassword"] ?? "Dent1@123";

        var created = await _commandDispatcher.Dispatch(
            new CreateUserCommand(
                command.Name.Trim(),
                email,
                username,
                phone,
                password,
                UserRole.Doctor,
                true),
            cancellationToken);

        DoctorSpecialtyRegistry.Set(created.Id, command.Specialty);
        return new DoctorReadModel(created.Id, created.Name, DoctorSpecialtyRegistry.Get(created.Id));
    }

    private async Task<string> ResolveUniqueUsernameAsync(string slug, CancellationToken cancellationToken)
    {
        for (var i = 0; i < 100; i++)
        {
            var candidate = i == 0 ? slug : $"{slug}{i}";
            if (!await _userRepository.ExistsByUsernameAsync(candidate, null, cancellationToken))
            {
                return candidate;
            }
        }

        return $"{slug}.{Guid.NewGuid():N}"[..24];
    }

    private async Task<string> ResolveUniquePhoneAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 50; i++)
        {
            var phone = $"9{Random.Shared.Next(100000000, 999999999)}";
            if (!await _userRepository.ExistsByPhoneNumberAsync(phone, null, cancellationToken))
            {
                return phone;
            }
        }

        return $"9{Random.Shared.Next(100000000, 999999999)}";
    }

    private static string ToUsernameSlug(string name)
    {
        var lowered = name.Trim().ToLowerInvariant();
        var slug = SlugRegex().Replace(lowered, ".");
        slug = Regex.Replace(slug, @"\.+", ".").Trim('.');
        return string.IsNullOrWhiteSpace(slug) ? $"doctor.{Guid.NewGuid():N}"[..16] : slug;
    }

    [GeneratedRegex(@"[^a-z0-9.]+")]
    private static partial Regex SlugRegex();
}
