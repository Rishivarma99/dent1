using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dent1.Data.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly DentContext _context;

    public TenantRepository(DentContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}