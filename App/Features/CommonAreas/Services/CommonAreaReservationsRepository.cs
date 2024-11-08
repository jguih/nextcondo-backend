using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreaReservationsRepository : IGenericRepository<CommonAreaReservation>
{
    public Task<List<CommonAreaReservation>> GetAsync(
        int? commonAreaId = null,
        DateOnly? date = null,
        int? slotId = null);

    public Task<List<CommonAreaReservationDTO>> GetDtoListAsync(Guid? userId = null, Guid? condominiumId = null);
}

public class CommonAreaReservationsRepository : GenericRepository<CommonAreaReservation>, ICommonAreaReservationsRepository
{
    public CommonAreaReservationsRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CommonAreaReservation>> logger)
        : base(context, logger)
    {
    }

    public async Task<List<CommonAreaReservation>> GetAsync(
        int? commonAreaId = null,
        DateOnly? date = null,
        int? slotId = null)
    {
        var hasCommonAreaId = commonAreaId.HasValue;
        var hasDate = date.HasValue;
        var hasSlotId = slotId.HasValue;
        var query = from reservation in entities
                    where (!hasCommonAreaId || reservation.CommonAreaId == commonAreaId)
                        && (!hasDate || reservation.Date == date)
                        && (!hasSlotId || reservation.SlotId == slotId)
                    select reservation;
        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<CommonAreaReservationDTO>> GetDtoListAsync(Guid? userId = null, Guid? condominiumId = null)
    {
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);
        var utcNow = DateTime.UtcNow;
        var timeNow = TimeOnly.FromDateTime(utcNow);
        var dateNow = DateOnly.FromDateTime(utcNow);
        var query = from reservation in entities
                    let commonArea = reservation.CommonArea
                    let commonAreaType = commonArea.Type
                    let slot = reservation.Slot
                    let endAt = reservation.StartAt.Add(commonArea.TimeInterval.ToTimeSpan())
                    let isInProgress = reservation.StartAt.CompareTo(timeNow) <= 0
                        && endAt.CompareTo(timeNow) > 0
                        && reservation.Date.CompareTo(dateNow) == 0
                    let isCompleted = reservation.Date.CompareTo(dateNow) < 0
                        || reservation.Date.CompareTo(dateNow) == 0
                        && reservation.StartAt.CompareTo(timeNow) < 0
                        && endAt.CompareTo(timeNow) < 0
                    let isConfirmed = reservation.Date.CompareTo(dateNow) > 0
                        || reservation.Date.CompareTo(dateNow) == 0
                        && reservation.StartAt.CompareTo(timeNow) > 0
                        && endAt.CompareTo(timeNow) > 0
                    let priority = isInProgress ? 1
                        : isConfirmed ? 2
                        : isCompleted ? 3
                        : 4
                    where (!hasUserId || reservation.UserId == userId)
                        && (!hasCondominiumId || commonArea.CondominiumId == condominiumId)
                    orderby priority, reservation.CreatedAt descending
                    select new CommonAreaReservationDTO()
                    {
                        Id = reservation.Id,
                        Date = reservation.Date,
                        StartAt = reservation.StartAt,
                        EndAt = reservation.StartAt.Add(commonArea.TimeInterval.ToTimeSpan()),
                        Status = isInProgress
                            ? "In Progress"
                            : isCompleted
                            ? "Completed"
                            : isConfirmed
                            ? "Confirmed"
                            : "",
                        CommonArea = new CommonAreaReservationDTOCommonArea()
                        {
                            Id = commonArea.Id,
                            Name_EN = commonAreaType.Name_EN,
                            Name_PTBR = commonAreaType.Name_PTBR
                        },
                        Slot = new CommonAreaReservationDTOCommonAreaSlot()
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
}