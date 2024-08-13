namespace NextCondoApi.Entity;

public class Role
{
    public required string Name { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}