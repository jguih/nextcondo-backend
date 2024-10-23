
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface IBookingSlotService
{
    public Task<BookingSlot> GetBookingSlotAsync(CommonArea commonArea, DateOnly date);
}

public class BookingSlotService : IBookingSlotService
{
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;
    private static readonly int MAX_TIME_SLOTS = 50;

    public BookingSlotService(ICommonAreaReservationsRepository commonAreaReservationsRepository)
    {
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
    }

    private bool ShouldCreateSlot(TimeOnly startAt, TimeOnly commonAreaEndTime, int index)
    {
        return startAt.CompareTo(commonAreaEndTime) < 0 && index != MAX_TIME_SLOTS;
    }

    public async Task<BookingSlot> GetBookingSlotAsync(CommonArea commonArea, DateOnly date)
    {
        var reservations = await _commonAreaReservationsRepository.GetAsync(commonArea.Id, date);

        BookingSlot bookingSlot = new()
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
            TimeSlot slot = new()
            {
                StartAt = startAt,
                Available = isAvailable,
            };
            bookingSlot.Slots.Add(slot);
            startAt = startAt.Add(commonArea.TimeInterval.ToTimeSpan());
            timeSlotsIndex++;
        }

        return bookingSlot;
    }
}