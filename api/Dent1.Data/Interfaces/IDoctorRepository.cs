using Dent1.Data.Entities;

namespace Dent1.Data.Interfaces;

public interface IDoctorRepository
{
    Task<Guid> AddAsync(string name, string specialty, CancellationToken cancellationToken);
    Task<List<Doctor>> GetAllAsync(CancellationToken cancellationToken);
    Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, string name, string specialty, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
