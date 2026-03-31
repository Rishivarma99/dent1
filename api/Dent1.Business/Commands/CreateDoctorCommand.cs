using Dent1.Business.Cqrs;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Commands;

public class CreateDoctorCommand : ICommand<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}

public class CreateDoctorCommandHandler : ICommandHandler<CreateDoctorCommand, Guid>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        var id = await _doctorRepository.AddAsync(command.Name, command.Specialty, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return id;
    }
}
