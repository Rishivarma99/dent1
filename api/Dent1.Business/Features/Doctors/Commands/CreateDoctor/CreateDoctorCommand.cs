using Dent1.Business.Abstractions;
using Dent1.Business.Features.Doctors.Models;

namespace Dent1.Business.Features.Doctors.Commands.CreateDoctor;

public sealed record CreateDoctorCommand(string Name, string Specialty) : ICommand<DoctorReadModel>;
