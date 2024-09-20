
namespace NextCondoApi.Entity;

public class OccurrenceType : BaseEntity
{
    public int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
    public string? Description_EN { get; set; }
    public string? Description_PTBR { get; set; }

    public override object GetId()
    {
        return Id;
    }
}