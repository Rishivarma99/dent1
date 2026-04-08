using Dent1.Business.Behaviors;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Commands;

public class DeleteUserCommand : IRequest<bool>, ITransactionalCommand
{
    public Guid Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _userRepository.SoftDeleteAsync(request.Id, cancellationToken);

        if (!deleted)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
