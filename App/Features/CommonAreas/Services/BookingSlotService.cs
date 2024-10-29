
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface IBookingSlotService
{
    public Task<BookingSlot> GetBookingSlotAsync(CommonArea commonArea, DateOnly date, int slotId);
}

public class BookingSlotService : IBookingSlotService
{
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;
    private static readonly int MAX_TIME_SLOTS = 50;

    public BookingSlotService(ICommonAreaReservationsRepository commonAreaReservationsRepository)
    {
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
    }

    private bool ShouldCreateTimeSlot(TimeOnly startAt, TimeOnly commonAreaEndTime, int index)
    {
        return startAt.CompareTo(commonAreaEndTime) < 0 && index != MAX_TIME_SLOTS;
    }

    private bool IsTimeSlotAvailable(List<CommonAreaReservation> reservations, TimeOnly startAt, DateOnly date)
    {
        TimeOnly timeUtcNow = TimeOnly.FromDateTime(DateTime.UtcNow);
        DateOnly dateUtcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var existingReservation = reservations
            .Find(reservation => reservation.StartAt.CompareTo(startAt) == 0);

        if (date.CompareTo(dateUtcNow) == 0)
        {
            return existingReservation is null && startAt.CompareTo(timeUtcNow) > 0;
        }

        return existingReservation is null;
    }

    public async Task<BookingSlot> GetBookingSlotAsync(CommonArea commonArea, DateOnly date, int slotId)
    {
        var reservations = await _commonAreaReservationsRepository.GetAsync(commonArea.Id, date, slotId);

        BookingSlot bookingSlot = new()
        {
            Date = date
        };

        var startAt = commonArea.StartTime;
        var timeSlotsIndex = 0;
        while (ShouldCreateTimeSlot(startAt, commonArea.EndTime, timeSlotsIndex))
        {
            TimeSlot slot = new()
            {
                StartAt = startAt,
                Available = IsTimeSlotAvailable(reservations, startAt, date),
            };
            bookingSlot.Slots.Add(slot);
            startAt = startAt.Add(commonArea.TimeInterval.ToTimeSpan());
            timeSlotsIndex++;
        }

        return bookingSlot;
    }
}