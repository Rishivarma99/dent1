using Dent1.Business.DTOs;
using Dent1.Data.Entities;
using Dent1.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DoctorsController(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var id = await _doctorRepository.AddAsync(request.Name, request.Specialty, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetAll(CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
        return Ok(doctors.Select(MapToDto).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);

        if (doctor is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(doctor));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var updated = await _doctorRepository.UpdateAsync(id, request.Name, request.Specialty, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _doctorRepository.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static DoctorDto MapToDto(Doctor doctor)
    {
        return new DoctorDto
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Specialty = doctor.Specialty
        };
    }

    public sealed class CreateDoctorRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }

    public sealed class UpdateDoctorRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
    }
}
