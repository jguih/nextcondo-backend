
using IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using NextCondoApi.Entity;

namespace IntegrationTests;

public class AuthTests
    : IClassFixture<TestsWebApplicationFactory<Program>>
{
    private readonly TestsWebApplicationFactory<Program> _factory;

    public AuthTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_LoginUser()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var db = provider.GetRequiredService<NextCondoApiDbContext>();
            var hasher = provider.GetRequiredService<IPasswordHasher<User>>();
            await DbUtils.AddTestUserAsync(db, hasher);
        };
        var client = _factory.CreateClient();
        var credentials = new FormUrlEncodedContent(
            new Dictionary<string, string>
                {
                    { "email", "test@test.com" },
                    { "password", "test12345" }
                });

        // Act
        var response = await client.PostAsync("/Auth/login", credentials);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        // Clean Resources
        using (var scope = _factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var db = provider.GetRequiredService<NextCondoApiDbContext>();
            await DbUtils.RemoveUsersAsync(db);
        };
    }

    [Fact]
    public async Task Post_RegisterNewUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        var user = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "email", "test2@test.com" },
                    { "password", "test12345" },
                    { "phone", "123456" },
                    { "fullName", "Testing user 2" }
                }
            );

        // Act
        var response = await client.PostAsync("/Auth/register", user);

        // Assert
        response.EnsureSuccessStatusCode();

        // Clean Resources
        using (var scope = _factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var db = provider.GetRequiredService<NextCondoApiDbContext>();
            await DbUtils.RemoveUsersAsync(db);
        };
    }
}