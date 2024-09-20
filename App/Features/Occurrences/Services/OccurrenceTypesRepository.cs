using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace NextCondoApi.Features.OccurrencesFeature.Services;

public interface IOccurrenceTypesRepository : IGenericRepository<OccurrenceType>
{
    public Task<List<OccurrenceTypeDTO>> GetDtoListAsync();
}

public class OccurrenceTypesRepository : GenericRepository<OccurrenceType>, IOccurrenceTypesRepository
{
    public OccurrenceTypesRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<OccurrenceType>> logger)
        : base(context, logger)
    {
    }

    public async Task<List<OccurrenceTypeDTO>> GetDtoListAsync()
    {
        var query = from type in entities
                    select new OccurrenceTypeDTO()
                    {
                        Id = type.Id,
                        Name_EN = type.Name_EN,
                        Name_PTBR = type.Name_PTBR,
                        Description_EN = type.Description_EN,
                        Description_PTBR = type.Description_PTBR
                    };
        var list = await query
            .AsNoTracking()
            .ToListAsync();
        return list;
    }
}