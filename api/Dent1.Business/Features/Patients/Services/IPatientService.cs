namespace Dent1.Business.Services;

public interface IPatientService
{
    Task<Guid> CreatePatientAsync(string name, string phone, CancellationToken cancellationToken);
}
