using System.Text.Json.Serialization;

namespace SimplifyCondoApi.Controllers.UsersControllerDTO;

public class CreateUserDTO
{
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