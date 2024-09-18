using Bogus;
using IntegrationTests.Utils;
using Newtonsoft.Json;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using System.Net;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class CondominiumTests : IClassFixture<TestsWebApplicationFactory<Program>>
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;

    public CondominiumTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task FailToAdd_WhenUserNotAuthenticated()
    {
        // Arrange
        var client = _factory.CreateClient();
        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(_faker.Company.CompanyName()), "name" },
            { new StringContent(_faker.Lorem.Paragraph(120)), "description" },
            { new StringContent(CondominiumUserRelationshipType.Manager.ToString()), "relationshipType" }
        };

        // Act
        var result = await client.PostAsync("/Condominium", newCondoDetails);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task AddNewCondominium()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;
        var testUser = authenticatedClient.user;
        var newCondoName = _faker.Company.CompanyName();
        var newCondoDescription = _faker.Lorem.Paragraph(3);
        var newCondoRelationshipType = CondominiumUserRelationshipType.Manager.ToString();

        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(newCondoName), "name" },
            { new StringContent(newCondoDescription), "description" },
            { new StringContent(newCondoRelationshipType), "relationshipType" }
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
        getMyCondoResult.EnsureSuccessStatusCode();
        Assert.Contains("application/json", getMyCondoResult.Content.Headers.ContentType?.ToString());
        Assert.NotNull(created);
        Assert.Equal(newCondoName, created.Name);
        Assert.Equal(newCondoDescription, created.Description);
        Assert.NotNull(userAsMember);
        Assert.Equal("Manager", userAsMember.RelationshipType);
    }

    [Fact]
    public async Task GetCurrent_Returns204_WhenUserHasNoCondominiums()
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
    public async Task GetCurrent_ReturnsCurrentCondominium_IfUserHasCondominiums()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;
        var testUser = authenticatedClient.user;
        var newCondoName = _faker.Company.CompanyName();
        var newCondoDescription = _faker.Lorem.Paragraph(3);
        var newCondoRelationshipType = CondominiumUserRelationshipType.Manager.ToString();

        using MultipartFormDataContent newCondoDetails = new()
        {
            { new StringContent(newCondoName), "name" },
            { new StringContent(newCondoDescription), "description" },
            { new StringContent(newCondoRelationshipType), "relationshipType" }
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
        Assert.Equal(newCondoName, current.Name);
        Assert.Equal(newCondoDescription, current.Description);
        Assert.Equal(testUser.Id, current.Owner.Id);
    }
}