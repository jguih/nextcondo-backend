namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class TimeSlot
{
    public TimeOnly StartAt { get; set; }
    public bool Available { get; set; } = true;
}