using Dent1.Api.Contracts.Requests.Auth;
using Dent1.Api.Contracts.Responses.Auth;
using Dent1.Business.Security;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login with username and password to get access and refresh tokens.
    /// Returns ApiResponse&lt;AuthResponse&gt; via the global response filter.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            new SignInRequest(request.UsernameOrPhone, request.Password),
            cancellationToken);

        if (result is null)
        {
            throw new AppException(Errors.Auth.InvalidCredentials);
        }

        return Ok(MapToAuthResponse(result));
    }

    /// <summary>
    /// Refresh an expired access token using a valid refresh token.
    /// Returns ApiResponse&lt;AuthResponse&gt; via the global response filter.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(
            new RefreshSessionRequest(request.RefreshToken),
            cancellationToken);

        if (result is null)
        {
            throw new AppException(Errors.Auth.InvalidRefreshToken);
        }

        return Ok(MapToAuthResponse(result));
    }

    private static AuthResponse MapToAuthResponse(AuthResult result) =>
        new()
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Role = result.Role,
            AccessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc
        };
}
