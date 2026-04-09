namespace Dent1.Business.Features.Patients.Queries.GetAllPatients;

public sealed record PatientReadModel(Guid Id, string Name, string Phone, DateTime CreatedAt);
