using Dent1.Business.Abstractions;
using Dent1.Business.Security;

namespace Dent1.Business.Features.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler(IAuthService authService) : ICommandHandler<LogoutCommand, bool>
{
    public Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        return authService.LogoutAsync(command.UserId, command.RefreshToken, cancellationToken);
    }
}
