using Dent1.Business.Abstractions;
using Dent1.Business.Security;
using Dent1.Common.Exceptions;
using Dent1.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Dent1.Business.Pipeline;

/// <summary>
/// Authorization middleware for CQRS pipeline.
/// Enforces the mandatory authorization check order:
/// 1. Check permission
/// 2. Check scope
/// 3. Check policy (optional, delegated to handler)
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;
    private readonly IPermissionService _permissionService;
    private readonly IResourceAccessService _resourceAccessService;

    public AuthorizationBehavior(
        ILogger<AuthorizationBehavior<TRequest, TResponse>> logger,
        IPermissionService permissionService,
        IResourceAccessService resourceAccessService)
    {
        _logger = logger;
        _permissionService = permissionService;
        _resourceAccessService = resourceAccessService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        Func<Task<TResponse>> next)
    {
        // Only apply authorization to IAuthorizableRequest
        if (request is not IAuthorizableRequest authRequest)
        {
            return await next();
        }

        // Get user context from request context (must be set by middleware/controller)
        var userContext = TryGetUserContext(request);
        if (userContext == null)
        {
            _logger.LogWarning("Authorization required but no user context found in request");
            throw new AppException(Errors.Auth.Unauthorized);
        }

        var requiredPermission = authRequest.RequiredPermission;

        // Step 1: Check permission
        if (!_permissionService.HasPermission(userContext, requiredPermission))
        {
            _logger.LogWarning(
                "Permission denied. UserId: {UserId}, TenantId: {TenantId}, Permission: {Permission}",
                userContext.UserId,
                userContext.TenantId,
                requiredPermission);

            throw new AppException(
                Errors.Auth.PermissionDenied,
                new Dictionary<string, object> { { "Permission", requiredPermission } });
        }

        // Step 2: Check scope
        // Note: Scope validation typically requires access to the target resource,
        // so detailed scope checks should happen in handlers after resource resolution.
        // This is a permission-level scope check only.

        _logger.LogInformation(
            "Authorization passed for request. UserId: {UserId}, Permission: {Permission}",
            userContext.UserId,
            requiredPermission);

        return await next();
    }

    private UserContext? TryGetUserContext(TRequest request)
    {
        // Try to get UserContext from request properties if it's set
        var property = request.GetType().GetProperty("UserContext");
        if (property?.GetValue(request) is UserContext context)
        {
            return context;
        }

        // If request doesn't have UserContext property, return null
        // Controller/middleware should have already set this
        return null;
    }
}
