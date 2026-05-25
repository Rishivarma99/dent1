using Dent1.Api.Contracts.Requests.Auth;
using Dent1.Api.Contracts.Responses.Auth;
using Dent1.Api.Controllers;
using Dent1.Business.Abstractions;
using Dent1.Business.Features.Authentication.Abstractions;
using Dent1.Business.Features.Authentication.Commands.Login;
using Dent1.Business.Features.Authentication.Commands.Logout;
using Dent1.Business.Features.Authentication.Commands.RefreshToken;
using Dent1.Business.Features.Authentication.Queries.GetCurrentAuthUser;
using Dent1.Common.MultiTenancy;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_returns_tokens_and_user_payload()
    {
        var commandDispatcher = new FakeCommandDispatcher
        {
            Response = new LoginResponse(
                "access-token",
                "refresh-token",
                new AuthenticatedUserDto(
                    Guid.Parse("8f7457f1-8b47-49e9-98b9-5442f9eb6977"),
                    "Alice Johnson",
                    "alice@dentova.com",
                    ["Admin"]))
        };

        var controller = new AuthController(commandDispatcher, new FakeQueryDispatcher(), new FakeCurrentUser(Guid.Empty, false));

        var actionResult = await controller.Login(new LoginRequest
        {
            UsernameOrPhone = "alice",
            Password = "password"
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.Equal("access-token", response.AccessToken);
        Assert.Equal("refresh-token", response.RefreshToken);
        Assert.Equal("Alice Johnson", response.User.Name);
        Assert.Equal("alice@dentova.com", response.User.Email);
        Assert.Equal(["Admin"], response.User.Roles);

        var command = Assert.IsType<LoginCommand>(commandDispatcher.LastCommand);
        Assert.Equal("alice", command.UsernameOrPhone);
        Assert.Equal("password", command.Password);
    }

    [Fact]
    public async Task Refresh_returns_rotated_token_pair_without_user_payload()
    {
        var commandDispatcher = new FakeCommandDispatcher
        {
            Response = new RefreshTokenResponse("next-access-token", "next-refresh-token")
        };

        var controller = new AuthController(commandDispatcher, new FakeQueryDispatcher(), new FakeCurrentUser(Guid.Empty, false));

        var actionResult = await controller.Refresh(new RefreshTokenRequest
        {
            RefreshToken = "refresh-token"
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<TokenPairResponse>(okResult.Value);

        Assert.Equal("next-access-token", response.AccessToken);
        Assert.Equal("next-refresh-token", response.RefreshToken);

        var command = Assert.IsType<RefreshTokenCommand>(commandDispatcher.LastCommand);
        Assert.Equal("refresh-token", command.RefreshToken);
    }

    [Fact]
    public async Task Me_returns_authenticated_user_for_current_principal()
    {
        var currentUserId = Guid.Parse("878d509f-4777-426f-9f8e-e3e69a8da718");
        var queryDispatcher = new FakeQueryDispatcher
        {
            Response = new AuthenticatedUserDto(
                currentUserId,
                "Alice Johnson",
                "alice@dentova.com",
                ["Admin"])
        };

        var controller = new AuthController(new FakeCommandDispatcher(), queryDispatcher, new FakeCurrentUser(currentUserId, true));

        var actionResult = await controller.Me(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<AuthUserResponse>(okResult.Value);

        Assert.Equal(currentUserId, response.Id);
        Assert.Equal("Alice Johnson", response.Name);
        Assert.Equal("alice@dentova.com", response.Email);
        Assert.Equal(["Admin"], response.Roles);

        var query = Assert.IsType<GetCurrentAuthUserQuery>(queryDispatcher.LastQuery);
        Assert.Equal(currentUserId, query.UserId);
    }

    [Fact]
    public async Task Logout_revokes_current_session_refresh_token()
    {
        var currentUserId = Guid.Parse("cc71d49f-f74e-420c-b8c8-3884ad36475d");
        var commandDispatcher = new FakeCommandDispatcher
        {
            Response = true
        };

        var controller = new AuthController(commandDispatcher, new FakeQueryDispatcher(), new FakeCurrentUser(currentUserId, true));

        var actionResult = await controller.Logout(new LogoutRequest
        {
            RefreshToken = "refresh-token"
        }, CancellationToken.None);

        Assert.IsType<NoContentResult>(actionResult);

        var command = Assert.IsType<LogoutCommand>(commandDispatcher.LastCommand);
        Assert.Equal(currentUserId, command.UserId);
        Assert.Equal("refresh-token", command.RefreshToken);
    }

    private sealed class FakeCommandDispatcher : ICommandDispatcher
    {
        public object? LastCommand { get; private set; }
        public object? Response { get; init; }

        public Task<TResponse> Dispatch<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            LastCommand = command;
            return Task.FromResult((TResponse)Response!);
        }
    }

    private sealed class FakeQueryDispatcher : IQueryDispatcher
    {
        public object? LastQuery { get; private set; }
        public object? Response { get; init; }

        public Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            LastQuery = query;
            return Task.FromResult((TResponse)Response!);
        }
    }

    private sealed class FakeCurrentUser(Guid userId, bool isAuthenticated) : ICurrentUser
    {
        public Guid UserId { get; } = userId;
        public bool IsAuthenticated { get; } = isAuthenticated;
        public IReadOnlyCollection<string> Permissions { get; } = Array.Empty<string>();
    }
}
