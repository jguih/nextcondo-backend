namespace NextCondoApi.Features.CommonAreasFeature.Models;

public class CommonAreaDTOSlot
{
    public int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
}

public class CommonAreaDTO
{
    public int Id { get; set; }
    public required CommonAreaTypeDTO Type { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly TimeInterval { get; set; }
    public IEnumerable<CommonAreaDTOSlot> Slots { get; set; } = [];
}