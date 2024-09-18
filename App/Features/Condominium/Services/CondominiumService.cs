using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public class CondominiumService
{
    private readonly ICurrentCondominiumRepository _currentCondominiumRepository;
    private readonly ICondominiumsRepository _condominiumsRepository;

    public CondominiumService(
        ICurrentCondominiumRepository currentCondominiumRepository, 
        ICondominiumsRepository condominiumsRepository)
    {
        _currentCondominiumRepository = currentCondominiumRepository;
        _condominiumsRepository = condominiumsRepository;
    }

    /// <summary>
    /// Returns the current condominium for user with userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<CondominiumDTO?> GetCurrentAsync(Guid userId)
    {
        var current = await _currentCondominiumRepository.GetDtoByUserIdAsync(userId);

        if (current is not null)
        {
            return current;
        }

        var firstCondominiumId = await _condominiumsRepository.GetFirstCondominiumIdForUser(userId);

        if (firstCondominiumId is null)
        {
            return null;
        }

        CurrentCondominium newCurrent = new()
        {
            CondominiumId = firstCondominiumId.Value,
            UserId = userId,
        };

        await _currentCondominiumRepository.AddAsync(newCurrent);

        return await _currentCondominiumRepository.GetDtoByUserIdAsync(userId);
    }
}
