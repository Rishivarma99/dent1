using Dent1.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Dent1.Api.Filters;

/// <summary>
/// Wraps controller results in ApiResponse&lt;T&gt; for consistent transport.
/// Success (2xx with body), client errors (4xx with body), and bare 404 are wrapped.
/// Skips: 204 No Content, already-wrapped responses, file/stream payloads.
/// </summary>
public sealed class ApiResponseFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (TryWrapSuccess(context))
        {
            return;
        }

        TryWrapError(context);
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    private static bool TryWrapSuccess(ResultExecutingContext context)
    {
        if (context.Result is not ObjectResult objectResult)
        {
            return false;
        }

        var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

        if (statusCode is < 200 or >= 300
            || objectResult.Value is null
            || IsAlreadyWrapped(objectResult.Value))
        {
            return false;
        }

        context.Result = CreateSuccessResult(objectResult.Value, statusCode);
        return true;
    }

    private static void TryWrapError(ResultExecutingContext context)
    {
        if (context.Result is StatusCodeResult { StatusCode: StatusCodes.Status404NotFound })
        {
            context.Result = CreateErrorResult(
                StatusCodes.Status404NotFound,
                "NOT_FOUND",
                "Resource not found");
            return;
        }

        if (context.Result is not ObjectResult objectResult)
        {
            return;
        }

        var statusCode = objectResult.StatusCode ?? StatusCodes.Status400BadRequest;

        if (statusCode is < 400 or >= 600 || IsAlreadyWrapped(objectResult.Value))
        {
            return;
        }

        var (code, message) = ExtractError(objectResult.Value, statusCode);
        context.Result = CreateErrorResult(statusCode, code, message);
    }

    private static ObjectResult CreateSuccessResult(object value, int statusCode) =>
        new(new ApiResponse<object>
        {
            Success = true,
            Data = value,
            Error = null
        })
        {
            StatusCode = statusCode
        };

    private static ObjectResult CreateErrorResult(int statusCode, string code, string message) =>
        new(new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Error = new ApiError
            {
                Code = code,
                Message = message
            }
        })
        {
            StatusCode = statusCode
        };

    private static (string Code, string Message) ExtractError(object? value, int statusCode)
    {
        if (value is string message && !string.IsNullOrWhiteSpace(message))
        {
            return (MapStatusCodeToErrorCode(statusCode), message);
        }

        if (value is ProblemDetails problemDetails)
        {
            var detail = problemDetails.Detail
                ?? problemDetails.Title
                ?? "Request failed";
            var code = problemDetails.Type;
            if (string.IsNullOrWhiteSpace(code))
            {
                code = MapStatusCodeToErrorCode(statusCode);
            }

            return (code, detail);
        }

        return (MapStatusCodeToErrorCode(statusCode), "Request failed");
    }

    private static string MapStatusCodeToErrorCode(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "BAD_REQUEST",
            StatusCodes.Status401Unauthorized => "UNAUTHORIZED",
            StatusCodes.Status403Forbidden => "FORBIDDEN",
            StatusCodes.Status404NotFound => "NOT_FOUND",
            StatusCodes.Status409Conflict => "CONFLICT",
            StatusCodes.Status422UnprocessableEntity => "VALIDATION_FAILED",
            _ => "REQUEST_FAILED"
        };

    private static bool IsAlreadyWrapped(object? value)
    {
        if (value is null)
        {
            return false;
        }

        var type = value.GetType();

        return type.IsGenericType &&
               type.GetGenericTypeDefinition() == typeof(ApiResponse<>);
    }
}
