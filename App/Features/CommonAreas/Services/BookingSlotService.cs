
using System.Globalization;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Utils;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface IBookingSlotService
{
    public Task<BookingSlot> GetBookingSlotAsync(
        CommonArea commonArea,
        DateOnly date,
        int slotId,
        int timezoneOffsetMinutes = 0);
}

public class BookingSlotService : IBookingSlotService
{
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;
    private static readonly int MAX_TIME_SLOTS = 50;
    /// <summary>
    /// Number of minutes required prior to making a reservation. 
    /// <para>
    /// Example: to book the schedule 11:30, the user's local time needs to be 10:59 or earlier with a tolerance of 30 minutes (default).
    /// </para>
    /// </summary>
    private static readonly int RESERVATION_TOLERANCE_MINUTES = 30;

    public BookingSlotService(ICommonAreaReservationsRepository commonAreaReservationsRepository)
    {
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
    }

    private bool ShouldCreateTimeSlot(TimeOnly startAt, TimeOnly endAt, int index)
    {
        return startAt != TimeOnly.MinValue
            && TimeZoneHelper.IsEarlierThan(startAt, endAt)
            && index != MAX_TIME_SLOTS;
    }

    private async Task<bool> IsTimeSlotAvailable(
        int commonAreaId,
        DateOnly date,
        int slotId,
        TimeOnly userStartAt,
        int timezoneOffsetMinutes = 0)
    {
        DateTime userNow = TimeZoneHelper.GetUserDateTime(timezoneOffsetMinutes);
        TimeOnly userTimeNow = TimeOnly.FromDateTime(userNow);
        DateOnly userDateNow = DateOnly.FromDateTime(userNow);
        var reservations = await _commonAreaReservationsRepository
            .GetAsync(commonAreaId, date, slotId);

        var existingReservation = reservations
            .Find(reservation =>
            {
                TimeOnly userReservationStartAt = TimeZoneHelper
                    .ConvertFromUTCToUserTime(
                        reservation.StartAt,
                        timezoneOffsetMinutes);
                return userReservationStartAt == userStartAt;
            });

        if (date == userDateNow)
        {
            var minBookingTime = userTimeNow.AddMinutes(RESERVATION_TOLERANCE_MINUTES);
            return existingReservation is null
                && userStartAt.CompareTo(minBookingTime) > 0;
        }

        return existingReservation is null;
    }

    public async Task<BookingSlot> GetBookingSlotAsync(
        CommonArea commonArea,
        DateOnly date,
        int slotId,
        int timezoneOffsetMinutes = 0)
    {
        var userStartAt = TimeZoneHelper.ConvertFromUTCToUserTime(commonArea.StartTime, timezoneOffsetMinutes);
        var userEndAt = TimeZoneHelper.ConvertFromUTCToUserTime(commonArea.EndTime, timezoneOffsetMinutes);
        var timeSlotsIndex = 0;
        BookingSlot bookingSlot = new()
        {
            Date = date
        };

        while (ShouldCreateTimeSlot(userStartAt, userEndAt, timeSlotsIndex))
        {
            TimeSlot slot = new()
            {
                StartAt = userStartAt,
                Available = await IsTimeSlotAvailable(
                    commonArea.Id,
                    date,
                    slotId,
                    userStartAt,
                    timezoneOffsetMinutes),
            };
            bookingSlot.Slots.Add(slot);
            userStartAt = userStartAt.Add(commonArea.TimeInterval.ToTimeSpan());
            timeSlotsIndex++;
        }

        return bookingSlot;
    }
}