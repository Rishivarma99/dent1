using Dent1.Business.Abstractions;

namespace Dent1.Business.Pipeline;

/// <summary>
/// Placeholder for future request-level authorization (e.g. scope).
/// Endpoint permission checks are enforced via <c>[HasPermission]</c> in the API layer.
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        return next();
    }
}
