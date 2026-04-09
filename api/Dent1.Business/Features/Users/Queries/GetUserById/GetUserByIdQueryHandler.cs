using Dent1.Business.Abstractions;
using Dent1.Business.Features.Users.Queries.GetAllUsers;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserReadModel?>
{
    private readonly DentContext _dbContext;

    public GetUserByIdQueryHandler(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserReadModel?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new UserReadModel(
                x.Id,
                x.Name,
                x.Email,
                x.Username,
                x.PhoneNumber,
                x.Role.ToString(),
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
