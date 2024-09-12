using NextCondoApi.Entity;
using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Models.DTO;

public class EditUserDTO
{
    [MaxLength(255)]
    public string? FullName { get; set; }
    [Phone]
    [MaxLength(30)]
    public string? Phone { get; set; }
}

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

    public static UserDTO FromUser(User user)
    {
        return new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = new() { Name = user.Role.Name }
        };
    }
}