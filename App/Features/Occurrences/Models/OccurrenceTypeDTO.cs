namespace NextCondoApi.Features.OccurrencesFeature.Models;

public class OccurrenceTypeDTO
{
    public required int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
    public string? Description_EN { get; set; }
    public string? Description_PTBR { get; set; }
}