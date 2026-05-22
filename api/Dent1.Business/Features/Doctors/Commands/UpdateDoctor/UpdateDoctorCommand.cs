using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Models;

namespace Dent1.Business.Features.Doctors.Commands.UpdateDoctor;

public sealed record UpdateDoctorCommand(Guid Id, string Name, string Specialty) : ICommand<DoctorReadModel?>;
