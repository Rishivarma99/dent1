using Dent1.Business.Abstractions;
using Dent1.Business.Features.Users.Queries.GetAllUsers;
using Dent1.Common.MultiTenancy;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserReadModel?>
{
    private readonly DentContext _dbContext;
    private readonly ICurrentTenant _currentTenant;

    public GetUserByIdQueryHandler(DentContext dbContext, ICurrentTenant currentTenant)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
    }

    public async Task<UserReadModel?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        if (!_currentTenant.IsResolved)
            throw new InvalidOperationException("Tenant not resolved.");

        return await _dbContext.Users
            .Where(x => x.Id == query.Id && x.TenantId == _currentTenant.TenantId)
            .AsNoTracking()
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
