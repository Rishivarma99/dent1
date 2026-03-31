using Dent1.Business.Cqrs;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Commands;

public class UpdateDoctorCommand : ICommand<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}

public class UpdateDoctorCommandHandler : ICommandHandler<UpdateDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDoctorCommand command, CancellationToken cancellationToken)
    {
        var updated = await _doctorRepository.UpdateAsync(command.Id, command.Name, command.Specialty, cancellationToken);

        if (!updated)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
