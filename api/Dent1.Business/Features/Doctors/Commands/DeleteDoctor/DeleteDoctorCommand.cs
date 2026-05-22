using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Doctors.Commands.DeleteDoctor;

public sealed record DeleteDoctorCommand(Guid Id) : ICommand<bool>;
