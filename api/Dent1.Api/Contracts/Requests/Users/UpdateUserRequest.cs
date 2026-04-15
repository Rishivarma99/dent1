using Dent1.Data.Enums;

namespace Dent1.Api.Contracts.Requests.Users;

public sealed record UpdateUserRequest(
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    UserRole Role,
    bool IsActive);
