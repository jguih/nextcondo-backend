namespace NextCondoApi.Entity;

public class CommonArea : BaseEntity
{
    public int Id { get; set; }
    public required int TypeId { get; set; }
    public CommonAreaType? Type { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly TimeInterval { get; set; }
    public Guid CondominiumId { get; set; }
    public Condominium? Condominium { get; set; }
    public ICollection<CommonAreaSlot> Slots { get; set; } = [];

    public override object GetId()
    {
        return Id;
    }
}