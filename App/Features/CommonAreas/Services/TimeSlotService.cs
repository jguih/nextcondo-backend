
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ITimeSlotService
{
    public Task<TimeSlot> GetTimeSlotAsync(CommonArea commonArea, DateOnly date);
}

public class TimeSlotService : ITimeSlotService
{
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;
    private static readonly int MAX_TIME_SLOTS = 50;

    public TimeSlotService(ICommonAreaReservationsRepository commonAreaReservationsRepository)
    {
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
    }

    private bool ShouldCreateSlot(TimeOnly startAt, TimeOnly commonAreaEndTime, int index)
    {
        return startAt.CompareTo(commonAreaEndTime) < 0 && index != MAX_TIME_SLOTS;
    }

    public async Task<TimeSlot> GetTimeSlotAsync(CommonArea commonArea, DateOnly date)
    {
        var reservations = await _commonAreaReservationsRepository.GetAsync(commonArea.Id, date);

        TimeSlot timeSlot = new()
        {
            Date = date
        };

        var startAt = commonArea.StartTime;
        var timeSlotsIndex = 0;
        while (ShouldCreateSlot(startAt, commonArea.EndTime, timeSlotsIndex))
        {
            var existingReservation = reservations
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

        return timeSlot;
    }
}