using System.Net;
using System.Net.Http.Json;
using IntegrationTests.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Utils;
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
        int timezoneOffsetMinutes = -180;
        var slotId = TestCommonArea.Slots.First().Id;
        DateTime date = TimeZoneHelper
            .GetUserDateTime(timezoneOffsetMinutes)
            .AddDays(1);
        var formattedDate = date.ToString("yyyy-MM-dd");
        var endpoint = $"/CommonAreas/{TestCommonArea.Id}/slot/{slotId}/date/{formattedDate}/bookingSlots?timezoneOffsetMinutes={timezoneOffsetMinutes}";
        var bookingSlotResult = await Client.GetAsync(endpoint);
        bookingSlotResult.EnsureSuccessStatusCode();
        var bookingSlot = await bookingSlotResult.Content.ReadFromJsonAsync<BookingSlot>();
        Assert.NotNull(bookingSlot);
        Assert.NotEmpty(bookingSlot.Slots);
        var firstTimeSlot = bookingSlot.Slots.First();
        Assert.NotNull(firstTimeSlot);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(bookingSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlot.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" },
            { new StringContent(slotId.ToString()), "slotId" },
            { new StringContent(timezoneOffsetMinutes.ToString()), "timezoneOffsetMinutes" }
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
        int timezoneOffsetMinutes = -180;
        var slotId = TestCommonArea.Slots.First().Id;
        DateTime date = TimeZoneHelper
            .GetUserDateTime(timezoneOffsetMinutes)
            .AddDays(2);
        var formattedDate = date.ToString("yyyy-MM-dd");
        var endpoint = $"/CommonAreas/{TestCommonArea.Id}/slot/{slotId}/date/{formattedDate}/bookingSlots?timezoneOffsetMinutes={timezoneOffsetMinutes}";
        var bookingSlotResult = await Client.GetAsync(endpoint);
        bookingSlotResult.EnsureSuccessStatusCode();
        var bookingSlot = await bookingSlotResult.Content.ReadFromJsonAsync<BookingSlot>();
        Assert.NotNull(bookingSlot);
        var firstTimeSlot = bookingSlot.Slots.First();
        Assert.NotNull(firstTimeSlot);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(bookingSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlot.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" },
            { new StringContent(slotId.ToString()), "slotId" },
            { new StringContent(timezoneOffsetMinutes.ToString()), "timezoneOffsetMinutes" }
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
        int timezoneOffsetMinutes = -180;
        var slotId = TestCommonArea.Slots.First().Id;
        DateTime date = TimeZoneHelper
            .GetUserDateTime(timezoneOffsetMinutes)
            .AddDays(3);
        var formattedDate = date.ToString("yyyy-MM-dd");
        var endpoint = $"/CommonAreas/{TestCommonArea.Id}/slot/{slotId}/date/{formattedDate}/bookingSlots?timezoneOffsetMinutes={timezoneOffsetMinutes}";
        var bookingSlotResult = await Client.GetAsync(endpoint);
        bookingSlotResult.EnsureSuccessStatusCode();
        var bookingSlot = await bookingSlotResult.Content.ReadFromJsonAsync<BookingSlot>();
        Assert.NotNull(bookingSlot);
        var firstTimeSlot = bookingSlot.Slots.First();
        Assert.NotNull(firstTimeSlot);
        firstTimeSlot.StartAt = firstTimeSlot.StartAt.AddMinutes(15);
        using MultipartFormDataContent newReservationDetails = new()
        {
            { new StringContent(bookingSlot.Date.ToString()), "date" },
            { new StringContent(firstTimeSlot.StartAt.ToString()), "startAt" },
            { new StringContent(TestCommonArea.Id.ToString()), "commonAreaId" },
            { new StringContent(slotId.ToString()), "slotId" },
            { new StringContent(timezoneOffsetMinutes.ToString()), "timezoneOffsetMinutes" }
        };

        // Act
        var result = await Client.PostAsync($"/CommonAreas/{TestCommonArea.Id}/reservation", newReservationDetails);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}