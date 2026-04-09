using Dent1.Business.Abstractions;
using Dent1.Business.Features.Patients.Queries.GetAllPatients;

namespace Dent1.Business.Features.Patients.Queries.GetPatientById;

public sealed record GetPatientByIdQuery(Guid Id) : IQuery<PatientReadModel?>;
