using Dent1.Business.Abstractions;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Common.MultiTenancy;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, bool>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(ICurrentTenant currentTenant, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _currentTenant = currentTenant;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
            throw new InvalidOperationException("Tenant not resolved.");

        if (await _userRepository.ExistsByUsernameAsync(command.Username, command.Id, cancellationToken))
        {
            throw new AppException(Errors.User.UsernameExists);
        }

        if (await _userRepository.ExistsByPhoneNumberAsync(command.PhoneNumber, command.Id, cancellationToken))
        {
            throw new AppException(Errors.User.PhoneExists);
        }

        var updated = await _userRepository.UpdateAsync(
            command.Id,
            command.Name.Trim(),
            command.Email.Trim(),
            command.Username.Trim(),
            command.PhoneNumber.Trim(),
            command.Role,
            _currentTenant.TenantId,
            command.IsActive,
            cancellationToken);

        if (!updated)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
