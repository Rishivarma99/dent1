using Dent1.Data.Entities;

namespace Dent1.Data.Interfaces;

public interface IPatientRepository
{
    Task<List<Patient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Patient>> SearchByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task<Guid> AddAsync(string name, string phone, CancellationToken cancellationToken);
}