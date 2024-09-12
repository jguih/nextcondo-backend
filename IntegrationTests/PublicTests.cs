using NextCondoApi;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class PublicTests : IClassFixture<TestsWebApplicationFactory<Program>>
{
    private readonly TestsWebApplicationFactory<Program> _factory;

    public PublicTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ApiStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Public/status");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}