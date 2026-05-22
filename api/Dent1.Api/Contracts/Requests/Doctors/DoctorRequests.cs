namespace Dent1.Api.Contracts.Requests.Doctors;

public sealed record CreateDoctorRequest(string Name, string Specialty);

public sealed record UpdateDoctorRequest(string Name, string Specialty);
