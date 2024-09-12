namespace NextCondoApi.Entity;

public class CondominiumUser : BaseEntity
{
    public required Guid UserId { get; set; }
    public required User User { get; set; }
    public required Guid CondominiumId { get; set; }
    public required Condominium Condominium { get; set; }
    public required CondominiumUserRelationshipType RelationshipType { get; set; }

    public override object GetId()
    {
        return new { UserId, CondominiumId, RelationshipType };
    }
}

public enum CondominiumUserRelationshipType
{
    Manager,
    Tenant
}
