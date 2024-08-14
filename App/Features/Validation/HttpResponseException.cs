using Microsoft.AspNetCore.Mvc;

namespace NextCondoApi.Features.Validation;

public class HttpResponseException : Exception
{
    public HttpResponseException(ProblemDetails details) =>
    (Details) = (details);
    public ProblemDetails Details { get; }
}
