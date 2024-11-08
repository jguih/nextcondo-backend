
using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreasRepository : IGenericRepository<CommonArea>
{
    public Task<CommonAreaDTO?> GetDtoAsync(int? id = null, Guid? condominiumId = null);
    public Task<List<CommonAreaDTO>> GetDtoListAsync(int? id = null, Guid? condominiumId = null, Guid? userId = null);
    public Task<CommonArea?> GetAsync(int? id = null, Guid? condominiumId = null);
    public Task<List<int>> GetExistingTypesId(Guid condominiumId);
}

public class CommonAreasRepository : GenericRepository<CommonArea>, ICommonAreasRepository
{
    public CommonAreasRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CommonArea>> logger)
         : base(context, logger)
    {
    }

    public async Task<CommonArea?> GetAsync(int? id = null, Guid? condominiumId = null)
    {
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);
        var hasId = id != null;
        var query = from commonArea in entities
                    where (!hasId || commonArea.Id == id)
                        && (!hasCondominiumId || commonArea.CondominiumId == condominiumId)
                    select commonArea;
        return await query
            .Include(ca => ca.Slots)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<CommonAreaDTO?> GetDtoAsync(int? id = null, Guid? condominiumId = null)
    {
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);
        var hasId = id != null;
        var query = from commonArea in entities
                    let type = commonArea.Type
                    let slots = commonArea.Slots
                    where (!hasId || commonArea.Id == id)
                        && (!hasCondominiumId || commonArea.CondominiumId == condominiumId)
                    select new CommonAreaDTO()
                    {
                        Id = commonArea.Id,
                        Type = new()
                        {
                            Id = type.Id,
                            Name_EN = type.Name_EN,
                            Name_PTBR = type.Name_PTBR
                        },
                        StartTime = commonArea.StartTime,
                        EndTime = commonArea.EndTime,
                        TimeInterval = commonArea.TimeInterval,
                        Slots = from slot in slots
                                select new CommonAreaDTOSlot()
                                {
                                    Id = slot.Id,
                                    Name_EN = slot.Name_EN,
                                    Name_PTBR = slot.Name_PTBR
                                }
                    };
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<List<CommonAreaDTO>> GetDtoListAsync(int? id = null, Guid? condominiumId = null, Guid? userId = null)
    {
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);
        var hasId = id != null;
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var query = from commonArea in entities
                    let type = commonArea.Type
                    let slots = commonArea.Slots
                    let condominium = commonArea.Condominium
                    let condominiumOwnerId = condominium.OwnerId
                    let condominiumMembers = condominium.Members
                    where (!hasId || commonArea.Id == id)
                        && (!hasCondominiumId || commonArea.CondominiumId == condominiumId)
                        && (!hasUserId
                            || condominiumOwnerId.Equals(userId)
                            || condominiumMembers.Any(m => m.UserId.Equals(userId)))
                    select new CommonAreaDTO()
                    {
                        Id = commonArea.Id,
                        Type = new()
                        {
                            Id = type.Id,
                            Name_EN = type.Name_EN,
                            Name_PTBR = type.Name_PTBR
                        },
                        StartTime = commonArea.StartTime,
                        EndTime = commonArea.EndTime,
                        TimeInterval = commonArea.TimeInterval,
                        Slots = from slot in slots
                                select new CommonAreaDTOSlot()
                                {
                                    Id = slot.Id,
                                    Name_EN = slot.Name_EN,
                                    Name_PTBR = slot.Name_PTBR
                                }
                    };
        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<int>> GetExistingTypesId(Guid condominiumId)
    {
        var query = from commonArea in entities
                    let type = commonArea.Type
                    where commonArea.CondominiumId == condominiumId
                    select type.Id;
        return await query
            .ToListAsync();
    }
}