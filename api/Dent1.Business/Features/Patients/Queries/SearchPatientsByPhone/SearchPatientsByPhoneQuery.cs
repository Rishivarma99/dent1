using Dent1.Business.Abstractions;
using Dent1.Business.Features.Patients.Queries.GetAllPatients;

namespace Dent1.Business.Features.Patients.Queries.SearchPatientsByPhone;

public sealed record SearchPatientsByPhoneQuery(string Phone) : IQuery<List<PatientReadModel>>;
