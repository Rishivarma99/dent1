using Dent1.Api.Contracts.Requests.Auth;
using Dent1.Api.Contracts.Responses.Auth;
using Dent1.Business.Abstractions;
using Dent1.Business.Features.Authentication.Abstractions;
using Dent1.Business.Features.Authentication.Commands.Login;
using Dent1.Business.Features.Authentication.Commands.Logout;
using Dent1.Business.Features.Authentication.Commands.RefreshToken;
using Dent1.Business.Features.Authentication.Queries.GetCurrentAuthUser;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Common.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICurrentUser _currentUser;

    public AuthController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ICurrentUser currentUser)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Login with username and password to get access and refresh tokens.
    /// Returns ApiResponse&lt;AuthResponse&gt; via the global response filter.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            new LoginCommand(request.UsernameOrPhone, request.Password),
            cancellationToken);
        return Ok(MapToAuthResponse(result));
    }

    /// <summary>
    /// Refresh an expired access token using a valid refresh token.
    /// Returns ApiResponse&lt;TokenPairResponse&gt; via the global response filter.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenPairResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            new RefreshTokenCommand(request.RefreshToken),
            cancellationToken);
        return Ok(MapToTokenPairResponse(result));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AuthUserResponse>> Me(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new AppException(Errors.Auth.Unauthorized);
        }

        var result = await _queryDispatcher.Dispatch(
            new GetCurrentAuthUserQuery(_currentUser.UserId),
            cancellationToken);
        if (result is null)
        {
            throw new AppException(Errors.Auth.Unauthorized);
        }

        return Ok(MapToAuthUserResponse(result));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new AppException(Errors.Auth.Unauthorized);
        }

        var revoked = await _commandDispatcher.Dispatch(
            new LogoutCommand(_currentUser.UserId, request.RefreshToken),
            cancellationToken);
        if (!revoked)
        {
            throw new AppException(Errors.Auth.InvalidRefreshToken);
        }

        return NoContent();
    }

    private static AuthResponse MapToAuthResponse(LoginResponse result) =>
        new()
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken,
            User = MapToAuthUserResponse(result.User)
        };

    private static TokenPairResponse MapToTokenPairResponse(RefreshTokenResponse result) =>
        new()
        {
            AccessToken = result.AccessToken,
            RefreshToken = result.RefreshToken
        };

    private static AuthUserResponse MapToAuthUserResponse(AuthenticatedUserDto result) =>
        new()
        {
            Id = result.Id,
            Name = result.Name,
            Email = result.Email,
            Roles = result.Roles
        };
}
