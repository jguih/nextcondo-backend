namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CommonAreaDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly TimeInterval { get; set; }
}