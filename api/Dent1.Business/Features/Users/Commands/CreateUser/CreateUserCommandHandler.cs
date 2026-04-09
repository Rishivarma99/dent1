using Dent1.Business.Abstractions;
using Dent1.Business.Security;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Entities;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
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

    public async Task<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByUsernameAsync(command.Username, null, cancellationToken))
        {
            throw new AppException(Errors.User.UsernameExists);
        }

        if (await _userRepository.ExistsByPhoneNumberAsync(command.PhoneNumber, null, cancellationToken))
        {
            throw new AppException(Errors.User.PhoneExists);
        }

        var passwordHash = _passwordService.HashPassword(new User(), command.Password);

        var id = await _userRepository.AddAsync(
            command.Name.Trim(),
            command.Email.Trim(),
            command.Username.Trim(),
            command.PhoneNumber.Trim(),
            passwordHash,
            command.Role,
            command.IsActive,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (created is null)
        {
            throw new AppException(Errors.User.NotFound, new Dictionary<string, object> { ["UserId"] = id });
        }

        return new CreateUserResponse(
            created.Id,
            created.Name,
            created.Email,
            created.Username,
            created.PhoneNumber,
            created.Role.ToString(),
            created.IsActive,
            created.CreatedAt,
            created.UpdatedAt);
    }
}
