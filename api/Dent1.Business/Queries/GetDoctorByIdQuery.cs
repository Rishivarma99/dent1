using Dent1.Business.Cqrs;
using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Queries;

public class GetDoctorByIdQuery : IQuery<DoctorDto?>
{
    public Guid Id { get; set; }

    public GetDoctorByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetDoctorByIdQueryHandler : IQueryHandler<GetDoctorByIdQuery, DoctorDto?>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetDoctorByIdQueryHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<DoctorDto?> Handle(GetDoctorByIdQuery query, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(query.Id, cancellationToken);

        if (doctor is null)
        {
            return null;
        }

        return new DoctorDto
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Specialty = doctor.Specialty
        };
    }
}
