using IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Models;
using System.Net;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class CondominiumTests : IClassFixture<TestsWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private HttpClient Client { get; set; } = null!;
    private User TestUser { get; set; } = null!;

    public CondominiumTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NextCondoApiDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        // Create Test User
        TestUser = await DbUtils.AddTestUserAsync(db, userDetails, hasher);
        // Create authenticated HttpClient
        Client = _factory.CreateClient();
        await Client.LoginAsync(userDetails.Email, userDetails.Password);
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Add_Condominium_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var details = FakeCondominiumsFactory.GetCondominiumDetails();
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(details.Name), "name" },
            { new StringContent(details.Description!), "description" },
            { new StringContent(details.RelationshipType.ToString()), "relationshipType" }
        };

        // Act
        var result = await client.PostAsync("/Condominium", newCondoDetails);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Add_Condominium_ReturnsCreatedCondominium()
    {
        // Arrange
        var details = FakeCondominiumsFactory.GetCondominiumDetails();
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(details.Name), "name" },
            { new StringContent(details.Description!), "description" },
            { new StringContent(details.RelationshipType.ToString()), "relationshipType" }
        };

        // Act
        var addCondoResult = await Client.PostAsync("/Condominium", newCondoDetails);
        var getMyCondoResult = await Client.GetAsync("/Condominium/mine");
        var getMyCondoResultBody = await getMyCondoResult.Content.ReadAsStringAsync();
        var myCondos = JsonConvert.DeserializeObject<List<CondominiumDTO>>(getMyCondoResultBody);
        var created = myCondos?.Find(condo => condo.Owner.Id.Equals(TestUser.Id));
        var userAsMember = created?.Members.First(m => m.Id.Equals(TestUser.Id));

        // Assert
        addCondoResult.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, addCondoResult.StatusCode);
        getMyCondoResult.EnsureSuccessStatusCode();
        Assert.Contains("application/json", getMyCondoResult.Content.Headers.ContentType?.ToString());
        Assert.NotNull(created);
        Assert.Equal(details.Name, created.Name);
        Assert.Equal(details.Description, created.Description);
        Assert.NotNull(userAsMember);
        Assert.Equal(details.RelationshipType.ToString(), userAsMember.RelationshipType);
    }

    [Fact]
    public async Task Get_Current_Returns204()
    {
        // Arrange

        // Act
        var result = await Client.GetAsync("/Condominium/mine/current");

        // Assert
        result.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Get_Current_ReturnsCurrentCondominium()
    {
        // Arrange
        var details = FakeCondominiumsFactory.GetCondominiumDetails();
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(details.Name), "name" },
            { new StringContent(details.Description!), "description" },
            { new StringContent(details.RelationshipType.ToString()), "relationshipType" }
        };

        // Act
        var addCondoResult = await Client.PostAsync("/Condominium", newCondoDetails);
        var currentResult = await Client.GetAsync("/Condominium/mine/current");
        var currentResultBody = await currentResult.Content.ReadAsStringAsync();
        var current = JsonConvert.DeserializeObject<CondominiumDTO>(currentResultBody);

        // Assert
        addCondoResult.EnsureSuccessStatusCode();
        currentResult.EnsureSuccessStatusCode();
        Assert.NotNull(current);
        Assert.Equal(details.Name, current.Name);
        Assert.Equal(details.Description, current.Description);
        Assert.Equal(TestUser.Id, current.Owner.Id);
    }
}