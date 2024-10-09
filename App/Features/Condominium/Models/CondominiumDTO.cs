
namespace NextCondoApi.Features.CondominiumFeature.Models;

public class CondominiumDTO
{
    public class CondominiumOwnerDTO
    {
        public required Guid Id { get; set; }
        public required string FullName { get; set; }
    }

    public class CondominiumMemberDTO
    {
        public required Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string RelationshipType { get; set; }
    }

    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required CondominiumOwnerDTO Owner { get; set; }
    public IEnumerable<CondominiumMemberDTO> Members { get; set; } = [];
}