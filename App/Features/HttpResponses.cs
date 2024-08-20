using System.Net.Mime;
using System.Text;
static class ResultsExtensions
{
    public static IResult NoContent(this IResultExtensions resultExtensions)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);
        return new NoContentResult();
    }
}

class NoContentResult : IResult
{
    public NoContentResult()
    {
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        return httpContext.Response.WriteAsJsonAsync(new { });
    }
}