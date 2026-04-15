using System.Security.Claims;

namespace Dent1.Business.Services;

public interface ITokenValidationService
{
    Task<TokenValidationResult> ValidateAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
}