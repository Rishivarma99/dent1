using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IQuery<List<UserReadModel>>;
