using System.Net.Http.Json;
using IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using TestFakes;

namespace IntegrationTests;

[Collection(nameof(TestsWebApplicationFactory<Program>))]
public class CommonAreaReservationsTests : IClassFixture<TestsWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestsWebApplicationFactory<Program> _factory;
    private HttpClient Client { get; set; } = null!;
    private User TestUser { get; set; } = null!;
    private Condominium TestCondo { get; set; } = null!;
    private CommonArea TestCommonArea { get; set; } = null!;

    public CommonAreaReservationsTests(TestsWebApplicationFactory<Program> factory)
    {
        _factory = factory;
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
        TestCommonArea = await DbUtils.AddCommonAreaAsync(
            db,
            FakeCommonAreasFactory.GetDetails(),
            TestCondo.Id);
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Create_Reservations_Returns_200()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;
        var now = new DateOnly(utcNow.Date.Year, utcNow.Date.Month, utcNow.Date.Day);

        // Act
        var timeSlotsResult = await Client.GetAsync($"/CommonAreas/{TestCommonArea.Id}/timeSlots");
        var timeSlots = await timeSlotsResult.Content.ReadFromJsonAsync<List<TimeSlot>>();
        var firstTimeSlot = timeSlots?.First();
        Assert.NotNull(firstTimeSlot);
        var firstTimeSlotSchedule = firstTimeSlot.Slots.First();
        Assert.NotNull(firstTimeSlotSchedule);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(firstTimeSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlotSchedule.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" }
        };
        var reservationResult = await Client.PostAsync("/CommonAreas/reservation", newReservationDetails);

        // Assert
        reservationResult.EnsureSuccessStatusCode();
    }
}