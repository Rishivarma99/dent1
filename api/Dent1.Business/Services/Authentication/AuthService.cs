using System.Security.Cryptography;
using System.Text;
using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Dent1.Business.Security;

public interface IAuthService
{
    Task<AuthSessionResult?> LoginAsync(SignInRequest request, CancellationToken cancellationToken);
    Task<TokenPairResult?> RefreshAsync(RefreshSessionRequest request, CancellationToken cancellationToken);
    Task<AuthenticatedUserResult?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
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

    public async Task<AuthSessionResult?> LoginAsync(SignInRequest request, CancellationToken cancellationToken)
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

        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, cancellationToken);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenHash = HashToken(refreshToken);

        user.RefreshTokenHash = refreshTokenHash;
        user.RefreshTokenCreatedAt = DateTime.UtcNow;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenLifetimeDays());
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthSessionResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            User: MapToAuthenticatedUser(user));
    }

    public async Task<TokenPairResult?> RefreshAsync(RefreshSessionRequest request, CancellationToken cancellationToken)
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

        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenPairResult(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken);
    }

    public async Task<AuthenticatedUserResult?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        return MapToAuthenticatedUser(user);
    }

    public async Task<bool> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.RefreshTokenHash))
        {
            return false;
        }

        if (!string.Equals(user.RefreshTokenHash, HashToken(refreshToken), StringComparison.Ordinal))
        {
            return false;
        }

        user.RefreshTokenHash = null;
        user.RefreshTokenCreatedAt = null;
        user.RefreshTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
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

    private static AuthenticatedUserResult MapToAuthenticatedUser(User user) =>
        new(
            user.Id,
            user.Name,
            user.Email,
            [user.Role.ToString()]);
}

public sealed record SignInRequest(string UsernameOrPhone, string Password);
public sealed record RefreshSessionRequest(string RefreshToken);
public sealed record AuthenticatedUserResult(
    Guid Id,
    string Name,
    string Email,
    IReadOnlyList<string> Roles);
public sealed record AuthSessionResult(
    string AccessToken,
    string RefreshToken,
    AuthenticatedUserResult User);
public sealed record TokenPairResult(
    string AccessToken,
    string RefreshToken);
