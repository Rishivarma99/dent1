using System.Security.Cryptography;
using System.Text;
using Dent1.Business.Security;
using Dent1.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Dent1.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly IPasswordService _passwordService;

    public AuthService(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _passwordService = passwordService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameOrPhoneAsync(request.UsernameOrPhone, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var passwordVerification = _passwordService.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (passwordVerification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordService.HashPassword(user, request.Password);
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = HashToken(refreshToken);

        user.RefreshTokenHash = refreshTokenHash;
        user.RefreshTokenCreatedAt = DateTime.UtcNow;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenLifetimeDays());
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Role = user.Role.ToString(),
            AccessTokenExpiresInSeconds = _jwtTokenService.GetAccessTokenLifetimeMinutes() * 60
        };
    }

    public async Task<AuthResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = HashToken(request.RefreshToken);
        var user = await _userRepository.GetByRefreshTokenHashAsync(refreshTokenHash, cancellationToken);

        if (user is null || !user.IsActive || user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshTokenHash = HashToken(newRefreshToken);
        user.RefreshTokenCreatedAt = DateTime.UtcNow;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenLifetimeDays());
        user.UpdatedAt = DateTime.UtcNow;

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            UserId = user.Id,
            Role = user.Role.ToString(),
            AccessTokenExpiresInSeconds = _jwtTokenService.GetAccessTokenLifetimeMinutes() * 60
        };
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private int GetRefreshTokenLifetimeDays()
    {
        var configured = _configuration["Jwt:RefreshTokenLifetimeDays"];
        return int.TryParse(configured, out var days) ? days : 7;
    }
}

public class LoginRequest
{
    public string UsernameOrPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public int AccessTokenExpiresInSeconds { get; set; }
}