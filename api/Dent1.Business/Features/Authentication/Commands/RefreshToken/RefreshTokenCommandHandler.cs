using Dent1.Business.Abstractions;
using Dent1.Business.Security;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;

namespace Dent1.Business.Features.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(IAuthService authService)
    : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var tokenPair = await authService.RefreshAsync(
            new RefreshSessionRequest(command.RefreshToken),
            cancellationToken);

        if (tokenPair is null)
        {
            throw new AppException(Errors.Auth.InvalidRefreshToken);
        }

        return new RefreshTokenResponse(tokenPair.AccessToken, tokenPair.RefreshToken);
    }
}
