using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Authentication.Commands.Logout;

public sealed record LogoutCommand(Guid UserId, string RefreshToken) : ICommand<bool>;
