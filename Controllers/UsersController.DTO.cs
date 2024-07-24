using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Controllers.UsersControllerDTO;

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
  public class UserRole
  {
    public required string Name { get; set; }
  }
  public Guid Id { get; set; }
  public required string Email { get; set; }
  public required UserRole Role { get; set; }
}