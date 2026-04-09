using Dent1.Business.Abstractions;
using Dent1.Data.Enums;

namespace Dent1.Business.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    string Password,
    UserRole Role,
    bool IsActive) : ICommand<CreateUserResponse>;
