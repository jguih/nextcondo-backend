using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public interface ICondominiumService
{
    /// <summary>
    /// Returns the current condominium for user with userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<CondominiumDTO?> GetCurrentAsync(Guid userId);
}

public class CondominiumService : ICondominiumService
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

    public async Task<CondominiumDTO?> GetCurrentAsync(Guid userId)
    {
        var current = await _currentCondominiumRepository.GetDtoFirstOrDefaultAsync(userId);

        if (current is not null)
        {
            return current;
        }

        var firstCondominiumId = await _condominiumsRepository.GetCondominiumIdFirstOrDefaultAsync(userId);

        if (!firstCondominiumId.HasValue || firstCondominiumId.Value.Equals(Guid.Empty))
        {
            return null;
        }

        CurrentCondominium newCurrent = new()
        {
            CondominiumId = firstCondominiumId.Value,
            UserId = userId,
        };

        await _currentCondominiumRepository.AddAsync(newCurrent);

        return await _currentCondominiumRepository.GetDtoFirstOrDefaultAsync(userId);
    }
}
