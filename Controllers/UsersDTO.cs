
namespace SimplifyCondoApi.Controllers.UsersDTO;

public class CreateUserDTO
{
}

public class UserDTO
{
  public Guid Id { get; set; }
  public required string Email { get; set; }
}