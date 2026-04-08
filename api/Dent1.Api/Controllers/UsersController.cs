using Dent1.Business.DTOs;
using Dent1.Business.Security;
using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Data.Entities;
using Dent1.Data.Enums;
using Dent1.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public UsersController(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var usernameExists = await _userRepository.ExistsByUsernameAsync(request.Username, null, cancellationToken);
        if (usernameExists)
        {
            throw new AppException(Errors.User.UsernameExists);
        }

        var phoneExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, null, cancellationToken);
        if (phoneExists)
        {
            throw new AppException(Errors.User.PhoneExists);
        }

        var passwordHash = _passwordService.HashPassword(new User(), request.Password);
        var id = await _userRepository.AddAsync(
            request.Name,
            request.Email,
            request.Username,
            request.PhoneNumber,
            passwordHash,
            request.Role,
            request.IsActive,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var result = users.Select(MapToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(user));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var usernameExists = await _userRepository.ExistsByUsernameAsync(request.Username, id, cancellationToken);
        if (usernameExists)
        {
            throw new AppException(Errors.User.UsernameExists);
        }

        var phoneExists = await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, id, cancellationToken);
        if (phoneExists)
        {
            throw new AppException(Errors.User.PhoneExists);
        }

        var updated = await _userRepository.UpdateAsync(
            id,
            request.Name,
            request.Email,
            request.Username,
            request.PhoneNumber,
            request.Role,
            request.IsActive,
            cancellationToken);

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
        var deleted = await _userRepository.SoftDeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public sealed class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}
