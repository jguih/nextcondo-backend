using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
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
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NextCondoApiDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var userDetails = FakeUsersFactory.GetFakeUserDetails();
        // Create Test User
        TestUser = await DbUtils.AddTestUserAsync(db, userDetails, hasher);
        // Create Test Condominium
        var condoDetails = FakeCondominiumsFactory.GetCondominiumDetails();
        condoDetails.OwnerId = TestUser.Id;
        TestCondo = await DbUtils.AddCondominiumAsync(db, condoDetails);
        // Create Test Common Area
        var commonAreaDetails = FakeCommonAreasFactory.GetDetails();
        commonAreaDetails.CondominiumId = TestCondo.Id;
        commonAreaDetails.TimeInterval = TimeOnly.Parse("01:00");
        TestCommonArea = await DbUtils.AddCommonAreaAsync(db, commonAreaDetails);
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
    public async Task Create_Reservations_Returns_200()
    {
        // Arrange

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

    [Fact]
    public async Task Create_SameReservationTwice_Returns_400()
    {
        // Arrange

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
        var firstResult = await Client.PostAsync("/CommonAreas/reservation", newReservationDetails);
        var secondResult = await Client.PostAsync("/CommonAreas/reservation", newReservationDetails);

        // Assert
        firstResult.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, secondResult.StatusCode);
    }

    [Fact]
    public async Task Create_ReservationWithInvalidSlot_Returns_400()
    {
        // Arrange

        // Act
        var timeSlotsResult = await Client.GetAsync($"/CommonAreas/{TestCommonArea.Id}/timeSlots");
        var timeSlots = await timeSlotsResult.Content.ReadFromJsonAsync<List<TimeSlot>>();
        var lastTimeSlot = timeSlots?.Last();
        Assert.NotNull(lastTimeSlot);
        var firstTimeSlotSchedule = lastTimeSlot.Slots.First();
        Assert.NotNull(firstTimeSlotSchedule);
        firstTimeSlotSchedule.StartAt = firstTimeSlotSchedule.StartAt.AddMinutes(15);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(lastTimeSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlotSchedule.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" }
        };
        var result = await Client.PostAsync("/CommonAreas/reservation", newReservationDetails);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}