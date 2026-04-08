using Dent1.Business.Behaviors;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Commands;

public class UpdateUserCommand : IRequest<bool>, ITransactionalCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var usernameExists = await _userRepository.ExistsByUsernameAsync(request.Username, request.Id, cancellationToken);
        if (usernameExists)
        {
            throw new AppException(Errors.User.UsernameExists);
        }

        var phoneExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, request.Id, cancellationToken);
        if (phoneExists)
        {
            throw new AppException(Errors.User.PhoneExists);
        }

        var updated = await _userRepository.UpdateAsync(
            request.Id,
            request.Name,
            request.Email,
            request.Username,
            request.PhoneNumber,
            request.Role,
            request.IsActive,
            cancellationToken);

        if (!updated)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
