namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class Slot
{
    public TimeOnly StartAt { get; set; }
    public bool Available { get; set; } = true;
}