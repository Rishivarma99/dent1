using Dent1.Business.Cqrs;
using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Queries;

public class GetAllDoctorsQuery : IQuery<List<DoctorDto>>
{
}

public class GetAllDoctorsQueryHandler : IQueryHandler<GetAllDoctorsQuery, List<DoctorDto>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetAllDoctorsQueryHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<List<DoctorDto>> Handle(GetAllDoctorsQuery query, CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepository.GetAllAsync(cancellationToken);

        return doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            Name = d.Name,
            Specialty = d.Specialty
        }).ToList();
    }
}
