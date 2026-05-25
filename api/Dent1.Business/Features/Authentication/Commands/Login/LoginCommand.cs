using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Authentication.Commands.Login;

public sealed record LoginCommand(string UsernameOrPhone, string Password) : ICommand<LoginResponse>;
