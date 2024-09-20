using IntegrationTests.Utils;
using Newtonsoft.Json;
using NextCondoApi;
using NextCondoApi.Models.DTO;
using System.Net;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class CondominiumTests : IClassFixture<TestsWebApplicationFactory<Program>>
{
    private readonly TestsWebApplicationFactory<Program> _factory;

    public CondominiumTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Add_Condominium_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var details = FakeCondominiumsFactory.GetFakeNewCondominiumDetails();
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
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;
        var testUser = authenticatedClient.user;
        var details = FakeCondominiumsFactory.GetFakeNewCondominiumDetails();
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(details.Name), "name" },
            { new StringContent(details.Description!), "description" },
            { new StringContent(details.RelationshipType.ToString()), "relationshipType" }
        };

        // Act
        var addCondoResult = await client.PostAsync("/Condominium", newCondoDetails);
        var getMyCondoResult = await client.GetAsync("/Condominium/mine");
        var getMyCondoResultBody = await getMyCondoResult.Content.ReadAsStringAsync();
        var myCondos = JsonConvert.DeserializeObject<List<CondominiumDTO>>(getMyCondoResultBody);
        var created = myCondos?.Find(condo => condo.Owner.Id.Equals(testUser.Id));
        var userAsMember = created?.Members.First(m => m.Id.Equals(testUser.Id));

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
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;

        // Act
        var result = await client.GetAsync("/Condominium/mine/current");

        // Assert
        result.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Get_Current_ReturnsCurrentCondominium()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;
        var testUser = authenticatedClient.user;
        var details = FakeCondominiumsFactory.GetFakeNewCondominiumDetails();
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(details.Name), "name" },
            { new StringContent(details.Description!), "description" },
            { new StringContent(details.RelationshipType.ToString()), "relationshipType" }
        };

        // Act
        var addCondoResult = await client.PostAsync("/Condominium", newCondoDetails);
        var currentResult = await client.GetAsync("/Condominium/mine/current");
        var currentResultBody = await currentResult.Content.ReadAsStringAsync();
        var current = JsonConvert.DeserializeObject<CondominiumDTO>(currentResultBody);

        // Assert
        addCondoResult.EnsureSuccessStatusCode();
        currentResult.EnsureSuccessStatusCode();
        Assert.NotNull(current);
        Assert.Equal(details.Name, current.Name);
        Assert.Equal(details.Description, current.Description);
        Assert.Equal(testUser.Id, current.Owner.Id);
    }
}