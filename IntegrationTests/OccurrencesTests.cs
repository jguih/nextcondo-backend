
using System.Net;
using Bogus;
using IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NextCondoApi;
using NextCondoApi.Entity;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class OccurrencesTests : IClassFixture<TestsWebApplicationFactory<Program>>
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;

    public OccurrencesTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task Get_OccurrenceTypes_ReturnsList()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;

        // Act
        var result = await client.GetAsync("/Occurrences/types");
        var resultBody = await result.Content.ReadAsStringAsync();
        var typeList = JsonConvert.DeserializeObject<List<OccurrenceTypeDTO>>(resultBody);

        result.EnsureSuccessStatusCode();
        Assert.NotNull(typeList);
        Assert.NotEmpty(typeList);
    }

    [Fact]
    public async Task Add_Occurrence_Returns404()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;
        var occurrenceDetails = FakeOccurrencesFactory.GetFakeNewOccurrenceDetails();
        using MultipartFormDataContent newOccurrence = new()
        {
            { new StringContent(occurrenceDetails.Title), "title" },
            { new StringContent(occurrenceDetails.Description!), "description" },
            { new StringContent(occurrenceDetails.OccurrenceTypeId.ToString()), "occurrenceTypeId" }
        };

        // Act
        var result = await client.PostAsync("/Occurrences", newOccurrence);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task Add_Occurrence_Returns201()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var authenticatedClient = await _factory.CreateAuthenticatedHttpClientForUserAsync(userDetails);
        var client = authenticatedClient.client;
        var testUser = authenticatedClient.user;
        Condominium condominium;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NextCondoApiDbContext>();
            condominium = await DbUtils.AddCondominiumAsync(
                db,
                testUser.Id,
                CondominiumUserRelationshipType.Tenant);
        }
        var occurrenceDetails = FakeOccurrencesFactory.GetFakeNewOccurrenceDetails();
        using MultipartFormDataContent newOccurrence = new()
        {
            { new StringContent(occurrenceDetails.Title), "title" },
            { new StringContent(occurrenceDetails.Description!), "description" },
            { new StringContent(occurrenceDetails.OccurrenceTypeId.ToString()), "occurrenceTypeId" }
        };

        // Act
        var result = await client.PostAsync("/Occurrences", newOccurrence);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }
}