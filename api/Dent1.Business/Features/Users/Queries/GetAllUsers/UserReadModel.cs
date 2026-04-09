namespace Dent1.Business.Features.Users.Queries.GetAllUsers;

public sealed record UserReadModel(
    Guid Id,
    string Name,
    string Email,
    string Username,
    string PhoneNumber,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
