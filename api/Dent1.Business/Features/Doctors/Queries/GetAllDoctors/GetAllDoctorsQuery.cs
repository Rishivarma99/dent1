using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Models;

namespace Dent1.Business.Features.Doctors.Queries.GetAllDoctors;

public sealed record GetAllDoctorsQuery : IQuery<List<DoctorReadModel>>;
