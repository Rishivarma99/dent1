using Dent1.Business.Commands;
using Dent1.Business.DTOs;
using Dent1.Business.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        command.Id = id;
        var updated = await _mediator.Send(command, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteUserCommand { Id = id }, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
