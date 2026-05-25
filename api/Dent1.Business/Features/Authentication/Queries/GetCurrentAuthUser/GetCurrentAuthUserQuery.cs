using Dent1.Business.Abstractions;
using Dent1.Business.Features.Authentication.Abstractions;

namespace Dent1.Business.Features.Authentication.Queries.GetCurrentAuthUser;

public sealed record GetCurrentAuthUserQuery(Guid UserId) : IQuery<AuthenticatedUserDto?>;
