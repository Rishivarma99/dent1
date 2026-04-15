using Dent1.Api.Contracts.Requests.Auth;
using Dent1.Api.Contracts.Responses.Auth;
using Dent1.Business.Security;
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
            return Unauthorized("Invalid credentials.");
        }

        return Ok(new AuthResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Role = result.Role,
            AccessTokenExpiresInSeconds = result.AccessTokenExpiresInSeconds
        });
    }

    /// <summary>
    /// Refresh an expired access token using a valid refresh token.
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
            return Unauthorized("Invalid or expired refresh token.");
        }

        return Ok(new AuthResponse
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            UserId = result.UserId,
            Role = result.Role,
            AccessTokenExpiresInSeconds = result.AccessTokenExpiresInSeconds
        });
    }
}
