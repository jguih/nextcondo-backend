using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreaTypesRepository : IGenericRepository<CommonAreaType>
{
    public Task<bool> Exists(int? Id = null);
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
}