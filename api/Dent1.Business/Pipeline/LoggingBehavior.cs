using Microsoft.Extensions.Logging;

namespace Dent1.Business.Pipeline;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        var requestType = typeof(TRequest).Name;
        var startedAt = DateTime.UtcNow;

        _logger.LogInformation("Handling {RequestType}", requestType);

        var response = await next();

        var elapsed = DateTime.UtcNow - startedAt;
        _logger.LogInformation("Handled {RequestType} in {ElapsedMs}ms", requestType, elapsed.TotalMilliseconds);

        return response;
    }
}
