namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class TimeSlot
{
    public DateOnly Date { get; set; }
    public List<Slot> Slots { get; set; } = [];
}