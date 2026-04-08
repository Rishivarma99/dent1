namespace Dent1.Common.Errors;

public class ErrorDefinition
{
    public required string Code { get; init; }
    public required string MessageTemplate { get; init; }
    public required int StatusCode { get; init; }
}
