using Dent1.Business.Features.Authentication.Abstractions;

namespace Dent1.Business.Features.Authentication.Commands.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    AuthenticatedUserDto User);
