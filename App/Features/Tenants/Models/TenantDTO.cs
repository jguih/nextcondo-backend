namespace NextCondoApi.Features.TenantsFeature.Models;

public class TenantDTO
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string RelationshipType { get; set; }
}