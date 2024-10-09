
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Features.OccurrencesFeature.Models;

namespace NextCondoApi.Features.OccurrencesFeature.Services;

public interface IOccurrencesService
{
    /// <summary>
    /// Creates a new occurrence for current condominium.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="userId"></param>
    /// <returns>
    /// Tuple with a number that represents why this function succeeded or failed and the created occurrence
    /// <para>(2, null) when user does not have a current condominium</para>
    /// <para>(1, null) when occurrence type does not exist</para>
    /// <para>(0, occurrence) on success</para>
    /// </returns>
    public Task<(int result, Occurrence? occurrence)> AddAsync(PostOccurrenceCommand data, Guid userId);
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
    public Task<(int result, OccurrenceDTO? occurrence)> GetByIdAsync(Guid occurrenceId, Guid userId);
    public Task<List<OccurrenceDTO>> GetListAsync(Guid userId);
    public Task<bool> DeleteAsync(Guid Id);
    /// <summary>
    /// Update an occurrence
    /// </summary>
    /// <param name="data"></param>
    /// <param name="userId"></param>
    /// <returns>
    /// Integer representing success or failure 
    /// <para>2 when user does not have a current condominium</para>
    /// <para>1 when occurrence does not exist</para>
    /// <para>0 on success</para>
    /// </returns>
    public Task<int> UpdateAsync(PutOccurrenceCommand data, Guid userId);
}

public class OccurrencesService : IOccurrencesService
{
    private readonly IOccurrencesRepository _occurrencesRepository;
    private readonly ICurrentCondominiumRepository _currentCondominiumRepository;
    private readonly IOccurrenceTypesRepository _occurrenceTypesRepository;

    public OccurrencesService(
        IOccurrencesRepository occurrencesRepository,
        ICurrentCondominiumRepository currentCondominiumRepository,
        IOccurrenceTypesRepository occurrenceTypesRepository)
    {
        _occurrencesRepository = occurrencesRepository;
        _currentCondominiumRepository = currentCondominiumRepository;
        _occurrenceTypesRepository = occurrenceTypesRepository;
    }

    public async Task<(int result, Occurrence? occurrence)> AddAsync(PostOccurrenceCommand data, Guid userId)
    {
        var current = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!current.HasValue || current.Value.Equals(Guid.Empty))
        {
            return (2, null);
        }

        var type = await _occurrenceTypesRepository.GetByIdAsync(data.OccurrenceTypeId);

        if (type is null)
        {
            return (1, null);
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
        return (0, occurrence);
    }

    public async Task<(int result, OccurrenceDTO? occurrence)> GetByIdAsync(Guid occurrenceId, Guid userId)
    {
        var current = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!current.HasValue || current.Value.Equals(Guid.Empty))
        {
            return (2, null);
        }

        var occurrence = await _occurrencesRepository.GetDtoAsync(occurrenceId, current.Value);

        if (occurrence is null)
        {
            return (1, null);
        }

        return (0, occurrence);
    }

    public async Task<List<OccurrenceDTO>> GetListAsync(Guid userId)
    {
        var current = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!current.HasValue || current.Value.Equals(Guid.Empty))
        {
            return [];
        }

        var occurrences = await _occurrencesRepository.GetDtoListAsync(current.Value);
        return occurrences;
    }

    public async Task<bool> DeleteAsync(Guid Id)
    {
        return await _occurrencesRepository.DeleteAsync(Id);
    }

    public async Task<int> UpdateAsync(PutOccurrenceCommand data, Guid userId)
    {
        if (!data.Id.HasValue || data.Id.Equals(Guid.Empty))
        {
            return 1;
        }

        var currentId = await _currentCondominiumRepository.GetCondominiumIdAsync(userId);

        if (!currentId.HasValue || currentId.Value.Equals(Guid.Empty))
        {
            return 2;
        }

        var occurrence = await _occurrencesRepository.GetByIdAsync(data.Id);

        if (occurrence is null || !occurrence.CondominiumId.Equals(currentId.Value))
        {
            return 1;
        }

        occurrence.Title = data.Title;
        occurrence.Description = data.Description;
        occurrence.OccurrenceTypeId = data.OccurrenceTypeId;

        await _occurrencesRepository.UpdateAsync(occurrence);
        return 0;
    }
}