namespace Dent1.Business.Features.Authentication.Abstractions;

public sealed record AuthenticatedUserDto(
    Guid Id,
    string Name,
    string Email,
    IReadOnlyList<string> Roles);
