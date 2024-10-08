
using System.Net;
using System.Net.Http.Json;
using Bogus;
using IntegrationTests.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Features.OccurrencesFeature.Models;
using NextCondoApi.Features.OccurrencesFeature.Services;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class OccurrencesTests : IClassFixture<TestsWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private readonly Faker _faker;
    private HttpClient Client { get; set; } = null!;
    private User TestUser { get; set; } = null!;
    private Condominium TestCondo { get; set; } = null!;

    public OccurrencesTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _faker = new Faker("pt_BR");
    }

    public async Task InitializeAsync()
    {
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        var (user, client) = await _factory.CreateAuthenticatedHttpClientAsync(userDetails);
        Client = client;
        TestUser = user;
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NextCondoApiDbContext>();
        TestCondo = await DbUtils.AddCondominiumAsync(
            db,
            TestUser.Id,
            CondominiumUserRelationshipType.Tenant);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Get_OccurrenceTypes_ReturnsList()
    {
        // Arrange

        // Act
        var result = await Client.GetAsync("/Occurrences/types");
        var typeList = await result.Content.ReadFromJsonAsync<List<OccurrenceTypeDTO>>();

        // Assert
        result.EnsureSuccessStatusCode();
        Assert.NotNull(typeList);
        Assert.NotEmpty(typeList);
    }

    [Fact]
    public async Task Add_Occurrence_Returns201()
    {
        // Arrange
        var occurrenceDetails = FakeOccurrencesFactory.GetFakeNewOccurrenceDetails();
        using MultipartFormDataContent newOccurrence = new()
        {
            { new StringContent(occurrenceDetails.Title), "title" },
            { new StringContent(occurrenceDetails.Description!), "description" },
            { new StringContent(occurrenceDetails.OccurrenceTypeId.ToString()), "occurrenceTypeId" }
        };

        // Act
        var result = await Client.PostAsync("/Occurrences", newOccurrence);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public async Task Get_OccurrenceById_Returns404()
    {
        // Arrange
        var occurrenceId = _faker.Random.Guid();

        // Act
        var result = await Client.GetAsync($"/Occurrences/{occurrenceId}");
        var resultBody = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.NotNull(resultBody);
    }

    [Fact]
    public async Task Get_OccurrenceById_Returns200()
    {
        // Arrange
        var occurrence = FakeOccurrencesFactory.GetFakeOccurrence();
        occurrence.CreatorId = TestUser.Id;
        occurrence.CondominiumId = TestCondo.Id;
        using var scope = _factory.Services.CreateScope();
        var occurrences = scope.ServiceProvider.GetRequiredService<IOccurrencesRepository>();
        await occurrences.AddAsync(occurrence);

        // Act
        var result = await Client.GetAsync($"/Occurrences/{occurrence.Id}");
        var resultBody = await result.Content.ReadFromJsonAsync<OccurrenceDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(resultBody);
    }

    [Fact]
    public async Task Get_Occurrence_ReturnsList()
    {
        // Arrange
        var occurrenceList = FakeOccurrencesFactory.GetFakeOccurrenceListBetween(10, 10);
        foreach (var occurrence in occurrenceList)
        {
            occurrence.CreatorId = TestUser.Id;
            occurrence.CondominiumId = TestCondo.Id;
        }
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NextCondoApiDbContext>();
        await db.Occurrences.AddRangeAsync(occurrenceList);
        await db.SaveChangesAsync();

        // Act
        var result = await Client.GetAsync($"/Occurrences");
        var resultBody = await result.Content.ReadFromJsonAsync<List<OccurrenceDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(resultBody);
        Assert.True(resultBody.Count >= 10);
    }
}