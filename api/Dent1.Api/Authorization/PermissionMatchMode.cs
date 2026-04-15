namespace Dent1.Api.Authorization;

public enum PermissionMatchMode
{
    /// <summary>User must have at least one of the listed permissions.</summary>
    Any = 1,

    /// <summary>User must have all listed permissions.</summary>
    All = 2
}
