namespace NextCondoApi.Entity;

public class Condominium : BaseEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public required User Owner { get; set; }
    public List<CondominiumUser> Members { get; set; } = [];

    public override object GetId()
    {
        return Id;
    }
}