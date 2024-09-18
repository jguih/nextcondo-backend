namespace NextCondoApi.Entity;

public class Condominium : BaseEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public ICollection<CondominiumUser> Members { get; set; } = [];

    public override object GetId()
    {
        return Id;
    }
}