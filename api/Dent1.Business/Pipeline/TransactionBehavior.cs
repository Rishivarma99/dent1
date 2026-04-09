using Dent1.Business.Abstractions;
using Dent1.Data.Interfaces;

namespace Dent1.Business.Pipeline;

public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        if (request is not ICommand<TResponse>)
        {
            return next();
        }

        return _unitOfWork.ExecuteInTransactionAsync(next, cancellationToken);
    }
}
