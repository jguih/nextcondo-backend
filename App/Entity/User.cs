

namespace SimplifyCondoApi.Entity;

public class User
{
  public Guid Id { get; set; }
  public required string Email { get; set; }
  public required string RoleId { get; set; }
  public Role? Role { get; set; }
}