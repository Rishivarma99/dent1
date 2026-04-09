using Dent1.Business.Abstractions;
using Dent1.Data.Enums;

namespace Dent1.Business.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    UserRole Role,
    bool IsActive) : ICommand<bool>;
