using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : ICommand<bool>;
