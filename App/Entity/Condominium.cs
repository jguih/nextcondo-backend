namespace NextCondoApi.Entity;

public class Condominium
{
    public Guid Id { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }

    public User? Owner { get; set; }
}