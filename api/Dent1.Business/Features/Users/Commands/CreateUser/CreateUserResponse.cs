namespace Dent1.Business.Features.Users.Commands.CreateUser;

public sealed record CreateUserResponse(
    Guid Id,
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
