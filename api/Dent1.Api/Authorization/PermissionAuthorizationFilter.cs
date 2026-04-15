using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dent1.Api.Authorization;

public sealed class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly PermissionMatchMode _matchMode;
    private readonly string[] _requiredPermissions;

    public PermissionAuthorizationFilter(PermissionMatchMode matchMode, string[] requiredPermissions)
    {
        _matchMode = matchMode;
        _requiredPermissions = requiredPermissions ?? Array.Empty<string>();
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            throw new AppException(Errors.Auth.Unauthorized);
        }

        if (_requiredPermissions.Length == 0)
        {
            return Task.CompletedTask;
        }

        var userPermissions = user
            .FindAll("permission")
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var allowed = _matchMode switch
        {
            PermissionMatchMode.All => _requiredPermissions.All(userPermissions.Contains),
            _ => _requiredPermissions.Any(userPermissions.Contains)
        };

        if (!allowed)
        {
            var needed = string.Join(", ", _requiredPermissions);
            throw new AppException(
                Errors.Auth.PermissionDenied,
                new Dictionary<string, object> { { "Permission", needed } });
        }

        return Task.CompletedTask;
    }
}
