using Dent1.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dent1.Api.Filters;

public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine(context) ; 
        if (context.Exception is not null)
        {
            return;
        }

        if (context.Result is not ObjectResult objectResult)
        {
            return;
        }

        var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
        if (statusCode < 200 || statusCode >= 300)
        {
            return;
        }

        if (objectResult.Value is null) // no content case 
        {
            return;
        }

        if (IsAlreadyWrapped(objectResult.Value))
        {
            return;
        }

        context.Result = new ObjectResult(new ApiResponse<object>
        {
            Success = true,
            Data = objectResult.Value,
            Error = null
        })
        {
            StatusCode = statusCode
        };
    }

    private static bool IsAlreadyWrapped(object value)
    {
        var type = value.GetType();

        if (!type.IsGenericType)
        {
            return false;
        }

        return type.GetGenericTypeDefinition() == typeof(ApiResponse<>);
    }
}
