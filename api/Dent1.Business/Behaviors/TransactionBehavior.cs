using Dent1.Data.Interfaces;
using MediatR;

namespace Dent1.Business.Behaviors;

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// MARKER INTERFACE — opt-in to DB transactions
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// Add this interface to any command that needs a transaction:
//
//   public class CreatePatientCommand : IRequest<Guid>, ITransactionalCommand { ... }
//
// Query handlers and commands that don't need transactions are
// unaffected — the behavior simply passes them straight through.
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public interface ITransactionalCommand { }

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// TRANSACTION PIPELINE BEHAVIOR
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// Runs AFTER ValidationBehavior so we only open a DB transaction
// once we know the input is valid.
//
// What it does:
//   1. Opens a DB transaction via IUnitOfWork
//   2. Calls the handler (next())
//   3. Commits on success
//   4. Rolls back automatically on any exception
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Skip transaction for queries and commands that don't need it
        if (request is not ITransactionalCommand)
            return await next();

        return await _unitOfWork.ExecuteInTransactionAsync(() => next(), cancellationToken);
    }
}
