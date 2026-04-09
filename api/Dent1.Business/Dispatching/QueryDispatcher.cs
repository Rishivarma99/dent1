using System.Reflection;
using Dent1.Business.Abstractions;
using Dent1.Business.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Dent1.Business.Dispatching;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var method = GetType().GetMethod(nameof(DispatchInternal), BindingFlags.NonPublic | BindingFlags.Instance);
        if (method is null)
        {
            throw new InvalidOperationException("DispatchInternal method was not found.");
        }

        var closedMethod = method.MakeGenericMethod(query.GetType(), typeof(TResponse));
        var result = closedMethod.Invoke(this, new object[] { query, cancellationToken });

        return (Task<TResponse>)result!;
    }

    private Task<TResponse> DispatchInternal<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TQuery, TResponse>>()
            .Where(x => !IsTransactionBehavior(x))
            .ToList();

        Func<Task<TResponse>> next = () => handler.Handle(query, cancellationToken);

        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var current = next;
            next = () => behavior.Handle(query, cancellationToken, current);
        }

        return next();
    }

    private static bool IsTransactionBehavior<TQuery, TResponse>(IPipelineBehavior<TQuery, TResponse> behavior)
    {
        var behaviorType = behavior.GetType();
        return behaviorType.IsGenericType && behaviorType.GetGenericTypeDefinition() == typeof(TransactionBehavior<,>);
    }
}
