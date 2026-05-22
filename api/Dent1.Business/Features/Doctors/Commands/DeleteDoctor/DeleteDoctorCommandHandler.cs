using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Data;
using Dent1.Business.Features.Users.Commands.DeleteUser;
using Dent1.Common.MultiTenancy;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Features.Doctors.Commands.DeleteDoctor;

public sealed class DeleteDoctorCommandHandler : ICommandHandler<DeleteDoctorCommand, bool>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentTenant _currentTenant;

    public DeleteDoctorCommandHandler(
        ICommandDispatcher commandDispatcher,
        IUserRepository userRepository,
        ICurrentTenant currentTenant)
    {
        _commandDispatcher = commandDispatcher;
        _userRepository = userRepository;
        _currentTenant = currentTenant;
    }

    public async Task<bool> Handle(DeleteDoctorCommand command, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
        {
            throw new InvalidOperationException("Tenant not resolved.");
        }

        var user = await _userRepository.GetByIdAsync(_currentTenant.TenantId, command.Id, cancellationToken);
        if (user is null || user.Role != UserRole.Doctor)
        {
            return false;
        }

        var deleted = await _commandDispatcher.Dispatch(new DeleteUserCommand(command.Id), cancellationToken);
        if (deleted)
        {
            DoctorSpecialtyRegistry.Remove(command.Id);
        }

        return deleted;
    }
}
