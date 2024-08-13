using Microsoft.AspNetCore.Diagnostics;

namespace NextCondoApi.Features.Validation;

public class GlobalExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(false);
    }
}
