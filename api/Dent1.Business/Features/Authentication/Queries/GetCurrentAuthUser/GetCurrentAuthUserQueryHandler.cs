using Dent1.Business.Abstractions;
using Dent1.Business.Features.Authentication.Abstractions;
using Dent1.Business.Security;

namespace Dent1.Business.Features.Authentication.Queries.GetCurrentAuthUser;

public sealed class GetCurrentAuthUserQueryHandler(IAuthService authService)
    : IQueryHandler<GetCurrentAuthUserQuery, AuthenticatedUserDto?>
{
    public async Task<AuthenticatedUserDto?> Handle(GetCurrentAuthUserQuery query, CancellationToken cancellationToken)
    {
        var user = await authService.GetCurrentUserAsync(query.UserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        return new AuthenticatedUserDto(user.Id, user.Name, user.Email, user.Roles);
    }
}
