using Dent1.Business.Abstractions;
using Dent1.Data;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Business.Services;

/// <summary>
/// Reads the current auth state for token validation.
/// Single lightweight query joining users and tenants.
/// </summary>
public sealed class TokenAuthStateReader : ITokenAuthStateReader
{
    private readonly DentContext _dbContext;

    public TokenAuthStateReader(DentContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TokenAuthState?> GetAuthStateAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return await (
            from user in _dbContext.Users
            join tenant in _dbContext.Tenants on user.TenantId equals tenant.Id
            where user.Id == userId && user.TenantId == tenantId
            select new TokenAuthState(
                user.Id,
                user.TenantId,
                user.IsActive,
                tenant.IsActive,
                user.SecurityStamp)
        ).FirstOrDefaultAsync(cancellationToken);
    }
}
