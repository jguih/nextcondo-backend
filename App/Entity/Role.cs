namespace SimplifyCondoApi.Entity;

public class Role
{
  public required string Name { get; set; }
  public ICollection<User> Users { get; set; } = new List<User>();
}