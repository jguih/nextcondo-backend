using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreaReservationsRepository : IGenericRepository<CommonAreaReservation>
{
    public Task<List<CommonAreaReservation>> GetAsync(int? commonAreaId = null);
}

public class CommonAreaReservationsRepository : GenericRepository<CommonAreaReservation>, ICommonAreaReservationsRepository
{
    public CommonAreaReservationsRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CommonAreaReservation>> logger)
        : base(context, logger)
    {
    }

    public async Task<List<CommonAreaReservation>> GetAsync(int? commonAreaId = null)
    {
        var hasCommonAreaId = commonAreaId.HasValue;
        var query = from reservation in entities
                    where !hasCommonAreaId || reservation.CommonAreaId == commonAreaId
                    select reservation;
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}