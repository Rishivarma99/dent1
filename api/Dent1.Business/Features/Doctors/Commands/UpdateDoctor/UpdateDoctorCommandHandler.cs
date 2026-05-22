using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Data;
using Dent1.Business.Features.Doctors.Models;
using Dent1.Business.Features.Users.Commands.UpdateUser;
using Dent1.Common.MultiTenancy;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Features.Doctors.Commands.UpdateDoctor;

public sealed class UpdateDoctorCommandHandler : ICommandHandler<UpdateDoctorCommand, DoctorReadModel?>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentTenant _currentTenant;

    public UpdateDoctorCommandHandler(
        ICommandDispatcher commandDispatcher,
        IUserRepository userRepository,
        ICurrentTenant currentTenant)
    {
        _commandDispatcher = commandDispatcher;
        _userRepository = userRepository;
        _currentTenant = currentTenant;
    }

    public async Task<DoctorReadModel?> Handle(UpdateDoctorCommand command, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
        {
            throw new InvalidOperationException("Tenant not resolved.");
        }

        var user = await _userRepository.GetByIdAsync(_currentTenant.TenantId, command.Id, cancellationToken);
        if (user is null || user.Role != UserRole.Doctor)
        {
            return null;
        }

        var updated = await _commandDispatcher.Dispatch(
            new UpdateUserCommand(
                user.Id,
                command.Name.Trim(),
                user.Email,
                user.Username,
                user.PhoneNumber,
                user.Role,
                user.IsActive),
            cancellationToken);

        if (!updated)
        {
            return null;
        }

        DoctorSpecialtyRegistry.Set(user.Id, command.Specialty);
        return new DoctorReadModel(user.Id, command.Name.Trim(), DoctorSpecialtyRegistry.Get(user.Id));
    }
}
