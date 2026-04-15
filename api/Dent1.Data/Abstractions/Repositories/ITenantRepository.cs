using Dent1.Data.Entities;

namespace Dent1.Data.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}