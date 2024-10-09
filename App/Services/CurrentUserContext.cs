
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
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
        var condoId = await _currentCondominiumRepository.GetCondominiumIdAsync(GetIdentity());
        if (condoId.HasValue)
        {
            return condoId.Value;
        }
        throw new ArgumentNullException(nameof(CurrentCondominium));
    }
}