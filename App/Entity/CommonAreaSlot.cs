namespace NextCondoApi.Entity;

public class CommonAreaSlot : BaseEntity
{
    public int Id { get; set; }
    public required string Name_EN { get; set; }
    public required string Name_PTBR { get; set; }
    public int CommonAreaId { get; set; }
    public CommonArea? CommonArea { get; set; }

    public override object GetId()
    {
        return Id;
    }
}