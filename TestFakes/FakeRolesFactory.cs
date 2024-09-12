using NextCondoApi.Entity;

namespace TestFakes;

public static class FakeRolesFactory
{
    private static Role DefaultRole { get; set; } = new()
    {
        Name = "User",
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    public static Role GetDefault()
    {
        return DefaultRole;
    }
}
