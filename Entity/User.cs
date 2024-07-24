

namespace NextCondoApi.Entity;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string RoleId { get; set; }
    public required string FullName { get; set; }
    public string? Phone { get; set; }

    public Role? Role { get; set; }
}