namespace NextCondoApi.Features.TenantsFeature.Models;

public class TenantDTO
{
    public class TenantRoleDTO
    {
        public required string Name { get; set; }
    }

    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required TenantRoleDTO Role { get; set; }
    public required string RelationshipType { get; set; }
}