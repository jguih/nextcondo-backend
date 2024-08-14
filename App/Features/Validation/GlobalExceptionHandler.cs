using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NextCondoApi.Features.Validation;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails details;
        if (exception is HttpResponseException)
        {
            details = ((HttpResponseException)exception).Details;
            logger.LogError(exception, details.Detail, exception.StackTrace);
        }
        else
        {
            details = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message,
                Title = "Internal server error",
                Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
            };
            logger.LogError(exception, exception.Message, exception.StackTrace);
        }
        httpContext.Response.StatusCode = details.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(details);

        return await ValueTask.FromResult(true);
    }
}
