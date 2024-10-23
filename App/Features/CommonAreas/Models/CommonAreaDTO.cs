namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CommonAreaDTO
{
    public class CommonAreaTypeDTO
    {
        public int Id { get; set; }
        public required string Name_EN { get; set; }
        public required string Name_PTBR { get; set; }
    }

    public int Id { get; set; }
    public required CommonAreaTypeDTO Type { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly TimeInterval { get; set; }
}