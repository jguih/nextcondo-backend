
namespace NextCondoApi.Features.UsersFeature.Models;

public class UserDTO
{
    public class UserRoleDTO
    {
        public required string Name { get; set; }
    }

    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required UserRoleDTO Role { get; set; }
}