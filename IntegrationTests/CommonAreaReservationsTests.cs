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
        var slotId = TestCommonArea.Slots.First().Id;
        var bookingSlotSlotsResult = await Client.GetAsync($"/CommonAreas/{TestCommonArea.Id}/slot/{slotId}/bookingSlots");
        var bookingSlots = await bookingSlotSlotsResult.Content.ReadFromJsonAsync<List<BookingSlot>>();
        var firstBookingSlot = bookingSlots?.First();
        Assert.NotNull(firstBookingSlot);
        var firstTimeSlot = firstBookingSlot.Slots.First();
        Assert.NotNull(firstTimeSlot);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(firstBookingSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlot.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" },
            { new StringContent(slotId.ToString()), "slotId" }
        };

        // Act
        var reservationResult = await Client.PostAsync($"/CommonAreas/{TestCommonArea.Id}/reservation", newReservationDetails);

        // Assert
        reservationResult.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Create_SameReservationTwice_Returns_400()
    {
        // Arrange
        var slotId = TestCommonArea.Slots.First().Id;
        var bookingSlotSlotsResult = await Client.GetAsync($"/CommonAreas/{TestCommonArea.Id}/slot/{slotId}/bookingSlots");
        var bookingSlots = await bookingSlotSlotsResult.Content.ReadFromJsonAsync<List<BookingSlot>>();
        var firstBookingSlot = bookingSlots?.First();
        Assert.NotNull(firstBookingSlot);
        var firstTimeSlot = firstBookingSlot.Slots.First();
        Assert.NotNull(firstTimeSlot);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(firstBookingSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlot.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" },
            { new StringContent(slotId.ToString()), "slotId" }
        };

        // Act
        var firstResult = await Client.PostAsync($"/CommonAreas/{TestCommonArea.Id}/reservation", newReservationDetails);
        var secondResult = await Client.PostAsync($"/CommonAreas/{TestCommonArea.Id}/reservation", newReservationDetails);

        // Assert
        firstResult.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, secondResult.StatusCode);
    }

    [Fact]
    public async Task Create_ReservationWithInvalidSlot_Returns_400()
    {
        // Arrange
        var slotId = TestCommonArea.Slots.First().Id;
        var bookingSlotSlotsResult = await Client.GetAsync($"/CommonAreas/{TestCommonArea.Id}/slot/{slotId}/bookingSlots");
        var bookingSlots = await bookingSlotSlotsResult.Content.ReadFromJsonAsync<List<BookingSlot>>();
        var firstBookingSlot = bookingSlots?.First();
        Assert.NotNull(firstBookingSlot);
        var firstTimeSlot = firstBookingSlot.Slots.First();
        Assert.NotNull(firstTimeSlot);
        firstTimeSlot.StartAt = firstTimeSlot.StartAt.AddMinutes(15);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(firstBookingSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlot.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" },
            { new StringContent(slotId.ToString()), "slotId" }
        };

        // Act
        var result = await Client.PostAsync($"/CommonAreas/{TestCommonArea.Id}/reservation", newReservationDetails);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}