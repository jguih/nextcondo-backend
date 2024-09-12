using Bogus;
using IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using System.Net;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class CondominiumTests : IClassFixture<TestsWebApplicationFactory<Program>>, IDisposable
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private HttpClient _httpClient = null!;
    private readonly Faker _faker;

    public CondominiumTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _faker = new Faker("pt_BR");
        _httpClient = _factory.CreateClient();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task FailToAdd_WhenUserNotAuthenticated()
    {
        // Arrange
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(_faker.Company.CompanyName()), "name" },
            { new StringContent(_faker.Lorem.Paragraph(120)), "description" },
            { new StringContent(_faker.Random.Guid().ToString()), "ownerId" },
            { new StringContent(CondominiumUserRelationshipType.Manager.ToString()), "relationshipType" }
        };

        // Act
        var result = await _httpClient.PostAsync("/Condominium", newCondoDetails);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task AddNewCondominium()
    {
        // Arrange
        RegisterUserDetails fakeUser = FakeUsersFactory.GetFakeUserDetails();
        Guid fakeUserId;
        using (var scope = _factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var users = provider.GetRequiredService<IUsersRepository>();
            var roles = provider.GetRequiredService<IRolesRepository>();
            var hasher = provider.GetRequiredService<IPasswordHasher<User>>();
            fakeUserId = await DbUtils.AddTestUserAsync(fakeUser, users, roles, hasher);
        };
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(_faker.Company.CompanyName()), "name" },
            { new StringContent(_faker.Lorem.Paragraph(3)), "description" },
            { new StringContent(fakeUserId.ToString()), "ownerId" },
            { new StringContent(CondominiumUserRelationshipType.Manager.ToString()), "relationshipType" }
        };

        // Act
        var loginResult = await _httpClient.LoginAsync(fakeUser.Email, fakeUser.Password);
        var addCondoResult = await _httpClient.PostAsync("/Condominium", newCondoDetails);

        // Assert
        loginResult.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, addCondoResult.StatusCode);
    }
}