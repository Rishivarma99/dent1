using Dent1.Common.Errors;

namespace Dent1.Common.Exceptions;

public class AppException : Exception
{
    public AppException(ErrorDefinition error, Dictionary<string, object>? @params = null)
    {
        Error = error;
        Params = @params;
    }

    public ErrorDefinition Error { get; }
    public Dictionary<string, object>? Params { get; }
}
