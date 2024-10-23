namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class BookingSlot
{
    public DateOnly Date { get; set; }
    public List<TimeSlot> Slots { get; set; } = [];
}