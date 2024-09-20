
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;

namespace NextCondoApi.Features.OccurrencesFeature.Services;

public class OccurrencesService
{
    private readonly IOccurrencesRepository _occurrencesRepository;
    private readonly ICondominiumsRepository _condominiumsRepository;
    private readonly ICurrentCondominiumRepository _currentCondominiumRepository;

    public OccurrencesService(
        IOccurrencesRepository occurrencesRepository,
        ICondominiumsRepository condominiumsRepository,
        ICurrentCondominiumRepository currentCondominiumRepository)
    {
        _occurrencesRepository = occurrencesRepository;
        _condominiumsRepository = condominiumsRepository;
        _currentCondominiumRepository = currentCondominiumRepository;
    }

    /// <summary>
    /// Creates a new occurrence for current condominium.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<Occurrence?> AddAsync(AddOccurrenceDTO data, Guid userId)
    {
        var current = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!current.HasValue || current.Value.Equals(Guid.Empty))
        {
            return null;
        }

        Occurrence occurrence = new()
        {
            CreatorId = userId,
            CondominiumId = current.Value,
            OccurrenceTypeId = data.OccurrenceTypeId,
            Title = data.Title,
            Description = data.Description,
        };

        await _occurrencesRepository.AddAsync(occurrence);
        return occurrence;
    }

    /// <summary>
    /// Get a occurrence by it's Id
    /// </summary>
    /// <param name="occurrenceId"></param>
    /// <param name="userId"></param>
    /// <returns>
    /// Tuple with a number that represents why this function succeeded or failed and the occurrence if it exists
    /// <para>(2, null) when user does not have a current condominium</para>
    /// <para>(1, null) when occurrence does not exist</para>
    /// <para>(0, occurrence) on success</para>
    /// </returns>
    public async Task<(int result, OccurrenceDTO? occurrence)> GetByIdAsync(Guid occurrenceId, Guid userId)
    {
        var current = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!current.HasValue || current.Value.Equals(Guid.Empty))
        {
            return (2, null);
        }

        var occurrence = await _occurrencesRepository.GetDtoFirstOrDefaultAsync(occurrenceId, current);

        if (occurrence is null)
        {
            return (1, null);
        }

        return (0, occurrence);
    }

    public async Task<List<OccurrenceDTO>> GetCurrentAsync(Guid userId)
    {
        var current = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!current.HasValue || current.Value.Equals(Guid.Empty))
        {
            return [];
        }

        var occurrences = await _occurrencesRepository.GetDtoListAsync(current.Value);
        return occurrences;
    }
}