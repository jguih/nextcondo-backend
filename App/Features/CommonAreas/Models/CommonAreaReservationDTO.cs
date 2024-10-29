namespace NextCondoApi.Features.CommonAreasFeature.Models;


public class CommonAreaReservationDTOCommonArea
{
    public int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
}

public class CommonAreaReservationDTOCommonAreaSlot
{
    public int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
}

public class CommonAreaReservationDTO
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartAt { get; set; }
    public TimeOnly EndAt { get; set; }
    public required string Status { get; set; }
    public required CommonAreaReservationDTOCommonArea CommonArea { get; set; }
    public required CommonAreaReservationDTOCommonAreaSlot Slot { get; set; }
}