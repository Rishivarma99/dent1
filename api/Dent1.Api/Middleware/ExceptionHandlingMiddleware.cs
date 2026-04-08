using Dent1.Common.Errors;
using Dent1.Common.Exceptions;
using Dent1.Common.Responses;

namespace Dent1.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Business error: {Code}", ex.Error.Code);
            await HandleAppExceptionAsync(context, ex);
        }
        catch (GenericException ex)
        {
            _logger.LogError(ex, "Generic exception");
            await HandleGenericExceptionAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleGenericExceptionAsync(context);
        }
    }

    private static async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.Error.StatusCode;

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Error = new ApiError
            {
                Code = ex.Error.Code,
                Message = ErrorFormatter.Format(ex.Error.MessageTemplate, ex.Params)
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleGenericExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = Errors.Common.InternalServerError.StatusCode;

        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Error = new ApiError
            {
                Code = Errors.Common.InternalServerError.Code,
                Message = Errors.Common.InternalServerError.MessageTemplate
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
