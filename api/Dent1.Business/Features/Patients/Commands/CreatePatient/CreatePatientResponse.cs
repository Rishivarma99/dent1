namespace Dent1.Business.Features.Patients.Commands.CreatePatient;

public sealed record CreatePatientResponse(Guid Id, string Name, string Phone, DateTime CreatedAt);
