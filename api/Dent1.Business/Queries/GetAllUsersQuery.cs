using Dent1.Business.DTOs;
using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Queries;

public class GetAllUsersQuery : IRequest<List<UserDto>>
{
}

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Username = u.Username,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role.ToString(),
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();
    }
}
