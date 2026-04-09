using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Patients.Queries.GetAllPatients;

public sealed record GetAllPatientsQuery : IQuery<List<PatientReadModel>>;
