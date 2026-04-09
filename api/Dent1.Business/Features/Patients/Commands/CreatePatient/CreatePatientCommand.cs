using Dent1.Business.Abstractions;

namespace Dent1.Business.Features.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(string Name, string Phone) : ICommand<CreatePatientResponse>;
