
using NextCondoApi.Features.TenantsFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.TenantsFeature.Services;

/// <summary>
/// Manages Tenants for current user's condominium.
/// </summary>
public interface ITenantsService
{
    public Task<List<TenantDTO>> GetListAsync();
}

public class TenantsService : ITenantsService
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICondominiumUserRepository _condominiumUserRepository;

    public TenantsService(
        ICurrentUserContext currentUserContext,
        ICondominiumUserRepository condominiumUserRepository)
    {
        _currentUserContext = currentUserContext;
        _condominiumUserRepository = condominiumUserRepository;
    }

    public async Task<List<TenantDTO>> GetListAsync()
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var list = await _condominiumUserRepository.GetTenantListAsync(currentCondoId);
        return list;
    }
}