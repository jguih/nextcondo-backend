using IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using NextCondoApi.Services;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class AuthTests : IClassFixture<TestsWebApplicationFactory<Program>>, IDisposable
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient = null!;

    public AuthTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Post_LoginUser()
    {
        // Arrange
        RegisterUserDetails testUser = FakeUsersFactory.GetFakeUserDetails();
        using (var scope = _factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var users = provider.GetRequiredService<IUsersRepository>();
            var roles = provider.GetRequiredService<IRolesRepository>();
            var hasher = provider.GetRequiredService<IPasswordHasher<User>>();
            await DbUtils.AddTestUserAsync(testUser, users, roles, hasher);
        };
        using MultipartFormDataContent credentials = new()
        {
            { new StringContent(testUser.Email), "email" },
            { new StringContent(testUser.Password), "password" }
        };

        // Act
        var response = await _httpClient.PostAsync("/Auth/login", credentials);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Post_RegisterNewUser()
    {
        // Arrange
        Role defaultRole;
        using (var scope = _factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var roles = provider.GetRequiredService<IRolesRepository>();
            defaultRole = await roles.GetDefaultAsync();
        };
        var user = FakeUsersFactory.GetFakeUserDetails();
        using MultipartFormDataContent register = new()
        {
            { new StringContent(user.Email), "email" },
            { new StringContent(user.Password), "password" },
            { new StringContent(user.Phone!), "phone" },
            { new StringContent(user.FullName), "fullName" }
        };
        using MultipartFormDataContent credentials = new()
        {
            { new StringContent(user.Email), "email" },
            { new StringContent(user.Password), "password" }
        };

        // Act
        var registerResponse = await _httpClient.PostAsync("/Auth/register", register);
        var loginResponse = await _httpClient.PostAsync("/Auth/login", credentials);
        var getMeResponse = await _httpClient.GetAsync("/Users/me");
        var getMeResponseBody = await getMeResponse.Content.ReadAsStringAsync();
        var createdUser = JsonConvert.DeserializeObject<UserDTO>(getMeResponseBody);

        // Assert
        registerResponse.EnsureSuccessStatusCode();
        loginResponse.EnsureSuccessStatusCode();
        getMeResponse.EnsureSuccessStatusCode();
        Assert.Contains("application/json", getMeResponse.Content.Headers.ContentType?.ToString());
        Assert.Equal(user.Email, createdUser?.Email);
        Assert.Equal(user.FullName, createdUser?.FullName);
        Assert.Equal(user.Phone, createdUser?.Phone);
        Assert.Equal(createdUser?.Role.Name, defaultRole.Name);
    }
}