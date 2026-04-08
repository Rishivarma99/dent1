namespace Dent1.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
}

public class ApiError
{
    public required string Code { get; set; }
    public required string Message { get; set; }
}
