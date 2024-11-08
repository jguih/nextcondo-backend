using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreaTypesRepository : IGenericRepository<CommonAreaType>
{
    public Task<bool> Exists(int? Id = null);
    public Task<List<CommonAreaTypeDTO>> GetDtoListAsync();
}

public class CommonAreaTypesRepository : GenericRepository<CommonAreaType>, ICommonAreaTypesRepository
{
    public CommonAreaTypesRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CommonAreaType>> logger)
        : base(context, logger)
    {
    }

    public async Task<bool> Exists(int? Id = null)
    {
        var hasId = Id.HasValue;
        var query = from type in entities
                    where !hasId || type.Id == Id
                    select 1;
        return await query.AnyAsync();
    }

    public async Task<List<CommonAreaTypeDTO>> GetDtoListAsync()
    {
        var query = from type in entities
                    select new CommonAreaTypeDTO()
                    {
                        Id = type.Id,
                        Name_EN = type.Name_EN,
                        Name_PTBR = type.Name_PTBR
                    };
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}