using Dent1.Business.Cqrs;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Commands;

public class DeleteDoctorCommand : ICommand<bool>
{
    public Guid Id { get; set; }
}

public class DeleteDoctorCommandHandler : ICommandHandler<DeleteDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteDoctorCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _doctorRepository.DeleteAsync(command.Id, cancellationToken);

        if (!deleted)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
