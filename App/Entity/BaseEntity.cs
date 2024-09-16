namespace NextCondoApi.Entity;

public class BaseEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Returns the id of this instance.
    /// Used for testing.
    /// </summary>
    /// <returns>Instance's Id</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual object GetId()
    {
        throw new NotImplementedException();
    }
}
