using Dent1.Api.Services;
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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        return Ok(result);
    }
}