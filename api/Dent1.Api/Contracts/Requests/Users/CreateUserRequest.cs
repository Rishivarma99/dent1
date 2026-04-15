using Dent1.Data.Enums;

namespace Dent1.Api.Contracts.Requests.Users;

public sealed record CreateUserRequest(
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    string Password,
    UserRole Role,
    bool IsActive = true);
