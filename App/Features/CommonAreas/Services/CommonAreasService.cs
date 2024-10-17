
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreasService
{
    public Task<int> AddAsync(CreateCommonAreaCommand data);
    public Task<List<TimeSlot>?> GetTimeSlotsAsync(int Id);
    public Task<CommonAreaDTO?> GetDtoAsync(int? Id);
    public Task<List<CommonAreaDTO>> GetDtoListAsync();
    public Task<int?> CreateReservationAsync(CreateReservationCommand data);
}

public class CommonAreasService : ICommonAreasService
{
    private readonly ICommonAreasRepository _commonAreasRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;

    public CommonAreasService(
        ICommonAreasRepository commonAreasRepository,
        ICurrentUserContext currentUserContext,
        ICommonAreaReservationsRepository commonAreaReservationsRepository)
    {
        _commonAreasRepository = commonAreasRepository;
        _currentUserContext = currentUserContext;
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
    }

    public async Task<int> AddAsync(CreateCommonAreaCommand data)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        CommonArea newCommonArea = new()
        {
            Name = data.Name,
            Description = data.Description,
            CondominiumId = currentCondoId,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            TimeInterval = data.TimeInterval
        };
        await _commonAreasRepository.AddAsync(newCommonArea);
        return newCommonArea.Id;
    }

    public async Task<int?> CreateReservationAsync(CreateReservationCommand data)
    {
        var identity = _currentUserContext.GetIdentity();
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetDtoAsync(
                id: data.CommonAreaId,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return null;
        }

        CommonAreaReservation newReservation = new()
        {
            UserId = identity,
            CommonAreaId = commonArea.Id,
            Date = data.Date,
            StartAt = data.StartAt
        };

        await _commonAreaReservationsRepository.AddAsync(newReservation);
        return newReservation.Id;
    }

    public async Task<CommonAreaDTO?> GetDtoAsync(int? Id)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        return await _commonAreasRepository
            .GetDtoAsync(
                id: Id,
                condominiumId: currentCondoId);
    }

    public async Task<List<CommonAreaDTO>> GetDtoListAsync()
    {
        var dtoList = await _commonAreasRepository.GetDtoListAsync();
        return dtoList;
    }

    public async Task<List<TimeSlot>?> GetTimeSlotsAsync(int Id)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetDtoAsync(
                id: Id,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return null;
        }

        var reservations = await _commonAreaReservationsRepository.GetAsync(commonArea.Id);
        int duration = 7;
        List<TimeSlot> timeSlotList = [];
        DateTime utcNow = DateTime.UtcNow;

        for (int i = 0; i < duration; i++)
        {
            var now = utcNow.AddDays(i);
            var date = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
            var reservationsForDate = reservations.FindAll(reservation => reservation.Date.CompareTo(date) == 0);

            TimeSlot timeSlot = new()
            {
                Date = date
            };

            var startAt = commonArea.StartTime;
            var maxTimeSlots = 50;
            var timeSlotsIndex = 0;
            while (startAt.CompareTo(commonArea.EndTime) < 0 && timeSlotsIndex != maxTimeSlots)
            {
                var existingReservation = reservationsForDate
                    .Find(reservation => reservation.StartAt.CompareTo(startAt) == 0);
                bool isAvailable = existingReservation is null;
                Slot slot = new()
                {
                    StartAt = startAt,
                    Available = isAvailable,
                };
                timeSlot.Slots.Add(slot);
                startAt = startAt.Add(commonArea.TimeInterval.ToTimeSpan());
                timeSlotsIndex++;
            }

            timeSlotList.Add(timeSlot);
        }

        return timeSlotList;
    }
}