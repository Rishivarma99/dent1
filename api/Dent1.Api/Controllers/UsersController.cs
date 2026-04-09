using Dent1.Business.Abstractions;
using Dent1.Business.Features.Users.Commands.CreateUser;
using Dent1.Business.Features.Users.Commands.DeleteUser;
using Dent1.Business.Features.Users.Commands.UpdateUser;
using Dent1.Business.Features.Users.Queries.GetAllUsers;
using Dent1.Business.Features.Users.Queries.GetUserById;
using Dent1.Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public UsersController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _commandDispatcher.Dispatch(
            new CreateUserCommand(
                request.Name,
                request.Email,
                request.Username,
                request.PhoneNumber,
                request.Password,
                request.Role,
                request.IsActive),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserReadModel>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _queryDispatcher.Dispatch(new GetAllUsersQuery(), cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserReadModel>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _queryDispatcher.Dispatch(new GetUserByIdQuery(id), cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var updated = await _commandDispatcher.Dispatch(
            new UpdateUserCommand(
                id,
                request.Name,
                request.Email,
                request.Username,
                request.PhoneNumber,
                request.Role,
                request.IsActive),
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _commandDispatcher.Dispatch(new DeleteUserCommand(id), cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    public sealed record CreateUserRequest(
        string Name,
        string Email,
        string Username,
        string PhoneNumber,
        string Password,
        UserRole Role,
        bool IsActive = true);

    public sealed record UpdateUserRequest(
        string Name,
        string Email,
        string Username,
        string PhoneNumber,
        UserRole Role,
        bool IsActive);
}
