namespace NextCondoApi.Entity;

public class CommonArea : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly TimeInterval { get; set; }
    public Guid CondominiumId { get; set; }
    public Condominium? Condominium { get; set; }
}