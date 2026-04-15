using Dent1.Business.Abstractions;
using Dent1.Common.MultiTenancy;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(ICurrentTenant currentTenant, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _currentTenant = currentTenant;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
            throw new InvalidOperationException("Tenant not resolved.");

        // Delete with tenant verification - ensures cross-tenant deletion is not possible
        var deleted = await _userRepository.DeleteAsync(_currentTenant.TenantId, command.Id, cancellationToken);
        if (!deleted)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
