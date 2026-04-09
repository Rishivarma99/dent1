using Dent1.Business.Abstractions;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Users.Queries.GetAllUsers;

public sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserReadModel>>
{
    private readonly DentContext _dbContext;

    public GetAllUsersQueryHandler(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserReadModel>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Name)
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
            .ToListAsync(cancellationToken);
    }
}
