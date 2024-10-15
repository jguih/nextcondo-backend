
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Features.Validation;
using NextCondoApi.Utils.ClaimsPrincipalExtension;

namespace NextCondoApi.Services;

public interface ICurrentUserContext
{
    public Guid GetIdentity();
    public Task<Guid> GetCurrentCondominiumIdAsync();
}

public class CurrentUserContext : ICurrentUserContext
{
    private readonly HttpContext _httpContext;
    private readonly ICurrentCondominiumRepository _currentCondominiumRepository;

    public CurrentUserContext(
        IHttpContextAccessor httpContextAccessor,
        ICurrentCondominiumRepository currentCondominiumRepository)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
        _currentCondominiumRepository = currentCondominiumRepository;
    }

    public Guid GetIdentity()
    {
        return _httpContext.User.GetIdentity();
    }

    public async Task<Guid> GetCurrentCondominiumIdAsync()
    {
        var condoId = await _currentCondominiumRepository.GetCondominiumIdAsync(userId: GetIdentity());
        if (condoId.HasValue && !condoId.Value.Equals(Guid.Empty))
        {
            return condoId.Value;
        }
        throw new HttpResponseException(new ProblemDetails()
        {
            Title = "Current condominium not found",
            Detail = "User does not have a current condominium set",
            Status = StatusCodes.Status404NotFound,
            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
        });
    }
}