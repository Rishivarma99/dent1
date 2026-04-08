using Dent1.Business.Behaviors;
using Dent1.Business.Security;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Commands;

public class CreateUserCommand : IRequest<Guid>, ITransactionalCommand
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var usernameExists = await _userRepository.ExistsByUsernameAsync(request.Username, null, cancellationToken);
        if (usernameExists)
        {
            throw new AppException(Errors.User.UsernameExists);
        }

        var phoneExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, null, cancellationToken);
        if (phoneExists)
        {
            throw new AppException(Errors.User.PhoneExists);
        }

        var passwordHash = _passwordService.HashPassword(new User(), request.Password);
        var id = await _userRepository.AddAsync(
            request.Name,
            request.Email,
            request.Username,
            request.PhoneNumber,
            passwordHash,
            request.Role,
            request.IsActive,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return id;
    }
}
