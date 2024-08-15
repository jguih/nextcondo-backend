using Microsoft.AspNetCore.Mvc.Testing;
using NextCondoApi;

namespace IntegrationTests;

public class StatusTests
    : IClassFixture<TestsWebApplicationFactory<Program>>
{
    private readonly TestsWebApplicationFactory<Program> _factory;

    public StatusTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ApiStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Status");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }
}