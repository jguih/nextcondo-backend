
using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace NextCondoApi.Features.OccurrencesFeature.Services;

public interface IOccurrencesRepository : IGenericRepository<Occurrence>
{
    public Task<OccurrenceDTO?> GetDtoFirstOrDefaultAsync(Guid? Id = default, Guid? condominiumId = default);
    public Task<List<OccurrenceDTO>> GetDtoListAsync(Guid? condominiumId = default);
}

public class OccurrencesRepository : GenericRepository<Occurrence>, IOccurrencesRepository
{
    public OccurrencesRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<Occurrence>> logger)
        : base(context, logger)
    {
    }

    public async Task<OccurrenceDTO?> GetDtoFirstOrDefaultAsync(Guid? Id, Guid? condominiumId)
    {
        var hasId = Id.HasValue && !Id.Value.Equals(Guid.Empty);
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);

        var query = from occurrence in entities
                    where (!hasId || occurrence.Id == Id)
                        && (!hasCondominiumId || occurrence.CondominiumId == condominiumId)
                    let occurrenceType = occurrence.OccurrenceType
                    let creator = occurrence.Creator
                    select new OccurrenceDTO()
                    {
                        Id = occurrence.Id,
                        Title = occurrence.Title,
                        Description = occurrence.Description,
                        OccurrenceType = new()
                        {
                            Id = occurrenceType.Id,
                            Name_EN = occurrenceType.Name_EN,
                            Name_PTBR = occurrenceType.Name_PTBR
                        },
                        Condominium = new()
                        {
                            Id = occurrence.CondominiumId
                        },
                        Creator = new()
                        {
                            Id = creator.Id,
                            FullName = creator.FullName
                        }
                    };

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<OccurrenceDTO>> GetDtoListAsync(Guid? condoId)
    {
        var hasCondoId = condoId.HasValue && !condoId.Value.Equals(Guid.Empty);

        var query = from occurrence in entities
                    let occurrenceType = occurrence.OccurrenceType
                    let creator = occurrence.Creator
                    where !hasCondoId || occurrence.CondominiumId == condoId
                    select new OccurrenceDTO()
                    {
                        Id = occurrence.Id,
                        Title = occurrence.Title,
                        Description = occurrence.Description,
                        CreatedAt = occurrence.CreatedAt,
                        UpdatedAt = occurrence.UpdatedAt,
                        OccurrenceType = new()
                        {
                            Id = occurrenceType.Id,
                            Name_EN = occurrenceType.Name_EN,
                            Name_PTBR = occurrenceType.Name_PTBR
                        },
                        Condominium = new()
                        {
                            Id = occurrence.CondominiumId
                        },
                        Creator = new()
                        {
                            Id = creator.Id,
                            FullName = creator.FullName
                        }
                    };

        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}