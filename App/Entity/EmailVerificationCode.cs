namespace NextCondoApi.Entity;

public class EmailVerificationCode : BaseEntity
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Code { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
    public required DateTimeOffset ExpirestAt { get; set; }

    public override object GetId()
    {
        return Id;
    }
}