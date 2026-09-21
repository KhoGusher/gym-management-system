using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GymSaaS.Api.Filters;

public record ApiResponse<T>(bool Success, T? Data, string? Message = null);

public class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // ProblemDetails (our error responses from GlobalExceptionHandler) already have their own
        // correct shape (RFC 7807) — don't double-wrap those, only wrap successful ObjectResults.
        if (context.Result is ObjectResult { Value: not ProblemDetails } objectResult)
        {
            var wrapped = typeof(ApiResponse<>)
                .MakeGenericType(objectResult.Value?.GetType() ?? typeof(object));
            var response = Activator.CreateInstance(wrapped, true, objectResult.Value, null);
            objectResult.Value = response;
        }

        await next();
    }
}