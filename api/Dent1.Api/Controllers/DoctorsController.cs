using Dent1.Business.Cqrs;
using Dent1.Business.Commands;
using Dent1.Business.DTOs;
using Dent1.Business.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly ICommandHandler<CreateDoctorCommand, Guid> _createHandler;
    private readonly IQueryHandler<GetAllDoctorsQuery, List<DoctorDto>> _getAllHandler;
    private readonly IQueryHandler<GetDoctorByIdQuery, DoctorDto?> _getByIdHandler;
    private readonly ICommandHandler<UpdateDoctorCommand, bool> _updateHandler;
    private readonly ICommandHandler<DeleteDoctorCommand, bool> _deleteHandler;

    public DoctorsController(
        ICommandHandler<CreateDoctorCommand, Guid> createHandler,
        IQueryHandler<GetAllDoctorsQuery, List<DoctorDto>> getAllHandler,
        IQueryHandler<GetDoctorByIdQuery, DoctorDto?> getByIdHandler,
        ICommandHandler<UpdateDoctorCommand, bool> updateHandler,
        ICommandHandler<DeleteDoctorCommand, bool> deleteHandler)
    {
        _createHandler = createHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetAll(CancellationToken cancellationToken)
    {
        var doctors = await _getAllHandler.Handle(new GetAllDoctorsQuery(), cancellationToken);
        return Ok(doctors);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await _getByIdHandler.Handle(new GetDoctorByIdQuery(id), cancellationToken);

        if (doctor is null)
        {
            return NotFound();
        }

        return Ok(doctor);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateDoctorCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var updated = await _updateHandler.Handle(command, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.Handle(new DeleteDoctorCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
