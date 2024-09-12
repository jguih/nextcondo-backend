namespace NextCondoApi.Entity;

public class BaseEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual object GetId()
    {
        throw new NotImplementedException();
    }
}
