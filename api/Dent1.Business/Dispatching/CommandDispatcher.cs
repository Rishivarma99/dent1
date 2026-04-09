using System.Reflection;
using Dent1.Business.Abstractions;
using Dent1.Business.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Dent1.Business.Dispatching;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Dispatch<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var method = GetType().GetMethod(nameof(DispatchInternal), BindingFlags.NonPublic | BindingFlags.Instance);
        if (method is null)
        {
            throw new InvalidOperationException("DispatchInternal method was not found.");
        }

        var closedMethod = method.MakeGenericMethod(command.GetType(), typeof(TResponse));
        var result = closedMethod.Invoke(this, new object[] { command, cancellationToken });

        return (Task<TResponse>)result!;
    }

    private Task<TResponse> DispatchInternal<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TCommand, TResponse>>()
            .ToList();

        Func<Task<TResponse>> next = () => handler.Handle(command, cancellationToken);

        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var current = next;
            next = () => behavior.Handle(command, cancellationToken, current);
        }

        return next();
    }
}
