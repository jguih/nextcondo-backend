
namespace NextCondoApi.Entity;

public class Occurrence : BaseEntity
{
    public Guid Id { get; set; }
    public required Guid CreatorId { get; set; }
    public User? Creator { get; set; }
    public required Guid CondominiumId { get; set; }
    public Condominium? Condominium { get; set; }
    public required int OccurrenceTypeId { get; set; }
    public OccurrenceType? OccurrenceType { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public override object GetId()
    {
        return Id;
    }
}