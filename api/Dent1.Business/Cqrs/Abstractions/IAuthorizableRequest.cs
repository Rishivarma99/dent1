namespace Dent1.Business.Abstractions;

/// <summary>
/// Marker interface for commands and queries that require authorization.
/// Implementing requests must explicitly declare their required permission.
/// </summary>
public interface IAuthorizableRequest
{
    /// <summary>
    /// The permission code required to execute this request.
    /// Must match a code from PermissionCodes.
    /// </summary>
    string RequiredPermission { get; }
}

/// <summary>
/// Marker interface for commands that require authorization.
/// </summary>
public interface IAuthorizableCommand : IAuthorizableRequest
{
}

/// <summary>
/// Marker interface for queries that require authorization.
/// </summary>
public interface IAuthorizableQuery : IAuthorizableRequest
{
}
