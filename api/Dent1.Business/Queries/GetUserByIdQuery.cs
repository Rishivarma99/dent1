using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Queries;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; set; }

    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return null;
        }

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
}
