namespace Dent1.Business.Abstractions;

public interface IQueryDispatcher
{
    Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
