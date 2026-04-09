using Dent1.Business.Abstractions;
using Dent1.Business.Features.Users.Queries.GetAllUsers;

namespace Dent1.Business.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserReadModel?>;
