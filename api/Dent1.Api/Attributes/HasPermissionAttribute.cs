using Microsoft.AspNetCore.Mvc;

namespace Dent1.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(params string[] permissions)
        : this(PermissionMatchMode.Any, permissions)
    {
    }

    public HasPermissionAttribute(PermissionMatchMode matchMode, params string[] permissions)
        : base(typeof(PermissionAuthorizationFilter))
    {
        Arguments = new object[] { matchMode, permissions };
        Order = 10;
    }
}
