namespace NextCondoApi.Entity;

public class CurrentCondominium : BaseEntity
{
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required Guid CondominiumId { get; set; }
    public Condominium? Condominium { get; set; }

    public override object GetId()
    {
        return new { UserId, CondominiumId };
    }
}
