using System.Security.Cryptography;
using System.Text;
using Dent1.Business.Security;
using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Dent1.Api.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_returns_authenticated_session_with_user_profile()
    {
        var user = CreateUser();
        var repository = new FakeUserRepository { UserByUsernameOrPhone = user };
        var unitOfWork = new FakeUnitOfWork();
        var jwtTokenService = new FakeJwtTokenService("access-token");
        var passwordService = new FakePasswordService(PasswordVerificationResult.Success);
        var authService = CreateAuthService(repository, unitOfWork, jwtTokenService, passwordService);

        var result = await authService.LoginAsync(new SignInRequest("alice", "password"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.AccessToken);
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Name, result.User.Name);
        Assert.Equal(user.Email, result.User.Email);
        Assert.Equal([user.Role.ToString()], result.User.Roles);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
        Assert.NotNull(user.RefreshTokenHash);
        Assert.NotNull(user.RefreshTokenExpiresAt);
    }

    [Fact]
    public async Task RefreshAsync_returns_rotated_token_pair()
    {
        var user = CreateUser();
        user.RefreshTokenHash = HashToken("old-refresh-token");
        user.RefreshTokenCreatedAt = DateTime.UtcNow.AddDays(-1);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1);

        var repository = new FakeUserRepository { UserByRefreshTokenHash = user };
        var unitOfWork = new FakeUnitOfWork();
        var jwtTokenService = new FakeJwtTokenService("new-access-token");
        var passwordService = new FakePasswordService(PasswordVerificationResult.Success);
        var authService = CreateAuthService(repository, unitOfWork, jwtTokenService, passwordService);

        var result = await authService.RefreshAsync(new RefreshSessionRequest("old-refresh-token"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("new-access-token", result.AccessToken);
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.NotEqual("old-refresh-token", result.RefreshToken);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
        Assert.NotEqual(HashToken("old-refresh-token"), user.RefreshTokenHash);
    }

    [Fact]
    public async Task GetCurrentUserAsync_returns_authenticated_user_for_active_user()
    {
        var user = CreateUser();
        var repository = new FakeUserRepository { UserById = user };
        var authService = CreateAuthService(
            repository,
            new FakeUnitOfWork(),
            new FakeJwtTokenService("unused"),
            new FakePasswordService(PasswordVerificationResult.Success));

        var result = await authService.GetCurrentUserAsync(user.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal([user.Role.ToString()], result.Roles);
    }

    [Fact]
    public async Task LogoutAsync_clears_refresh_token_fields_when_refresh_token_matches()
    {
        var user = CreateUser();
        user.RefreshTokenHash = HashToken("refresh-token");
        user.RefreshTokenCreatedAt = DateTime.UtcNow.AddDays(-1);
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1);

        var repository = new FakeUserRepository { UserById = user };
        var unitOfWork = new FakeUnitOfWork();
        var authService = CreateAuthService(
            repository,
            unitOfWork,
            new FakeJwtTokenService("unused"),
            new FakePasswordService(PasswordVerificationResult.Success));

        var loggedOut = await authService.LogoutAsync(user.Id, "refresh-token", CancellationToken.None);

        Assert.True(loggedOut);
        Assert.Null(user.RefreshTokenHash);
        Assert.Null(user.RefreshTokenCreatedAt);
        Assert.Null(user.RefreshTokenExpiresAt);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    private static AuthService CreateAuthService(
        FakeUserRepository repository,
        FakeUnitOfWork unitOfWork,
        FakeJwtTokenService jwtTokenService,
        FakePasswordService passwordService)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:RefreshTokenLifetimeDays"] = "7"
            })
            .Build();

        return new AuthService(repository, unitOfWork, jwtTokenService, configuration, passwordService);
    }

    private static User CreateUser() =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Name = "Alice Johnson",
            Email = "alice@dentova.com",
            Username = "alice",
            PhoneNumber = "9999999999",
            PasswordHash = "hashed-password",
            Role = UserRole.Admin,
            IsActive = true,
            SecurityStamp = "stamp"
        };

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public User? UserByUsernameOrPhone { get; init; }
        public User? UserByRefreshTokenHash { get; init; }
        public User? UserById { get; init; }

        public Task<Guid> AddAsync(string name, string email, string username, string phoneNumber, string passwordHash, UserRole role, Guid tenantId, bool isActive, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(UserById);

        public Task<User?> GetByIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
            => Task.FromResult(UserById);

        public Task<bool> UpdateAsync(Guid id, string name, string email, string username, string phoneNumber, UserRole role, Guid tenantId, bool isActive, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<bool> DeleteAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<bool> ExistsByUsernameAsync(string username, Guid? excludingUserId, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, Guid? excludingUserId, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<User?> GetByUsernameOrPhoneAsync(string usernameOrPhone, CancellationToken cancellationToken)
            => Task.FromResult(UserByUsernameOrPhone);

        public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken)
        {
            if (UserByRefreshTokenHash is null || UserByRefreshTokenHash.RefreshTokenHash != refreshTokenHash)
            {
                return Task.FromResult<User?>(null);
            }

            return Task.FromResult<User?>(UserByRefreshTokenHash);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
            => await action();
    }

    private sealed class FakeJwtTokenService(params string[] accessTokens) : IJwtTokenService
    {
        private readonly Queue<string> _accessTokens = new(accessTokens);

        public Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken)
            => Task.FromResult(_accessTokens.Count > 0 ? _accessTokens.Dequeue() : "generated-access-token");

        public int GetAccessTokenLifetimeMinutes() => 30;
    }

    private sealed class FakePasswordService(PasswordVerificationResult result) : IPasswordService
    {
        public string HashPassword(User user, string password) => $"rehash:{password}";

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
            => result;
    }
}
