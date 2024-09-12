namespace NextCondoApi.Entity;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    public override string GetId()
    {
        return Name;
    }
}