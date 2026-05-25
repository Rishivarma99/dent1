using Dent1.Business.Abstractions;
using Dent1.Business.Features.Authentication.Abstractions;
using Dent1.Business.Security;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;

namespace Dent1.Business.Features.Authentication.Commands.Login;

public sealed class LoginCommandHandler(IAuthService authService) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var session = await authService.LoginAsync(
            new SignInRequest(command.UsernameOrPhone, command.Password),
            cancellationToken);

        if (session is null)
        {
            throw new AppException(Errors.Auth.InvalidCredentials);
        }

        return new LoginResponse(
            session.AccessToken,
            session.RefreshToken,
            MapUser(session.User));
    }

    private static AuthenticatedUserDto MapUser(AuthenticatedUserResult user) =>
        new(user.Id, user.Name, user.Email, user.Roles);
}
