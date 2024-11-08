
using Bogus;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Features.CommonAreasFeature.Services;
using NextCondoApi.Services;
using NextCondoApi.Utils;
using Org.BouncyCastle.Asn1.Cms;
using TestFakes;

namespace UnitTests;

public class CommonAreaReservationsTests
{
    private readonly CommonAreasService _commonAreasService;
    private readonly BookingSlotService _timeSlotService;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<ICommonAreasRepository> _commonAreasRepositoryMock;
    private readonly Mock<ICommonAreaReservationsRepository> _commonAreaReservationsRepositoryMock;
    private readonly Mock<ICommonAreaTypesRepository> _commonAreaTypesRepositoryMock;
    private readonly Faker _faker;

    public CommonAreaReservationsTests()
    {
        _currentUserContextMock = new Mock<ICurrentUserContext>();
        _commonAreasRepositoryMock = new Mock<ICommonAreasRepository>();
        _commonAreaReservationsRepositoryMock = new Mock<ICommonAreaReservationsRepository>();
        _commonAreaReservationsRepositoryMock
            .Setup(mock => mock.GetAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<int>()))
            .Returns(Task.FromResult<List<CommonAreaReservation>>([]));
        _commonAreaTypesRepositoryMock = new Mock<ICommonAreaTypesRepository>();
        _faker = new Faker();
        _timeSlotService = new(_commonAreaReservationsRepositoryMock.Object);
        _commonAreasService = new(
            _commonAreasRepositoryMock.Object,
            _currentUserContextMock.Object,
            _commonAreaReservationsRepositoryMock.Object,
            _commonAreaTypesRepositoryMock.Object,
            _timeSlotService);
    }

    [Fact]
    public async Task GetTimeSlots_Returns_CommonAreaNotFound()
    {
        // Arrange
        int timezoneOffsetMinutes = -180;
        var commonAreaId = _faker.Random.Int();
        var condominiumId = _faker.Random.Guid();
        var slotId = _faker.Random.Int();
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonAreaId, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(null));

        // Act
        var (result, bookingSlots) = await _commonAreasService
            .GetBookingSlotsAsync(commonAreaId, slotId, timezoneOffsetMinutes);

        // Assert
        Assert.Equal(GetBookingSlotsResult.CommonAreaNotFound, result);
        Assert.Null(bookingSlots);
    }

    [Fact]
    public async Task GetTimeSlots_Returns_TimeSlotsArray()
    {
        // Arrange
        int timezoneOffsetMinutes = -180;
        var condominiumId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        var slotId = commonArea.Slots.First().Id;
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(commonArea));

        // Act
        var (result, bookingSlots) = await _commonAreasService
            .GetBookingSlotsAsync(commonArea.Id, slotId, timezoneOffsetMinutes);

        // Assert
        Assert.Equal(GetBookingSlotsResult.Ok, result);
        Assert.NotNull(bookingSlots);
        Assert.NotEmpty(bookingSlots);
        Assert.Equal(7, bookingSlots.Count);
        Assert.NotEmpty(bookingSlots.First().Slots);
    }

    [Fact]
    public async Task Create_Reservation_Returns_CommonAreaNotFound()
    {
        // Arrange
        int timezoneOffsetMinutes = -180;
        var condominiumId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        var commonAreaId = _faker.Random.Int();
        var slotId = _faker.Random.Int();
        var now = DateTime.UtcNow;
        var today = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
        _currentUserContextMock
            .Setup(mock => mock.GetIdentity())
            .Returns(userId);
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonAreaId, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(null));
        CreateReservationCommand data = new()
        {
            Date = today,
            StartAt = TimeOnly.Parse("01:00"),
            SlotId = slotId,
            TimezoneOffsetMinutes = timezoneOffsetMinutes
        };

        // Act
        var (result, reservationId) = await _commonAreasService.CreateReservationAsync(commonAreaId, data);

        // Assert
        Assert.Equal(CreateReservationResult.CommonAreaNotFound, result);
        Assert.Null(reservationId);
    }

    [Fact]
    public async Task Create_Reservation_Returns_InvalidTimeSlot()
    {
        // Arrange
        int timezoneOffsetMinutes = -180;
        var condominiumId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        var slotId = commonArea.Slots.First().Id;
        var now = DateTime.UtcNow;
        var today = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
        _currentUserContextMock
            .Setup(mock => mock.GetIdentity())
            .Returns(userId);
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(commonArea));
        CreateReservationCommand data = new()
        {
            Date = today,
            StartAt = TimeOnly.Parse("01:30"),
            SlotId = slotId,
            TimezoneOffsetMinutes = timezoneOffsetMinutes
        };

        // Act
        var (result, reservationId) = await _commonAreasService.CreateReservationAsync(commonArea.Id, data);

        // Assert
        Assert.Equal(CreateReservationResult.InvalidTimeSlot, result);
        Assert.Null(reservationId);
    }

    [Fact]
    public async Task Create_Reservation_Returns_UnavailableTimeSlot()
    {
        // Arrange
        int timezoneOffsetMinutes = -180;
        var condominiumId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        commonArea.StartTime = new TimeOnly(6, 0);
        commonArea.EndTime = new TimeOnly(22, 0);
        commonArea.TimeInterval = new TimeOnly(1, 0);
        var slotId = commonArea.Slots.First().Id;
        var today = TimeZoneHelper.GetUserDateTime(timezoneOffsetMinutes);
        var reservationTime = new TimeOnly(today.AddHours(1).Hour, 0);
        _currentUserContextMock
            .Setup(mock => mock.GetIdentity())
            .Returns(userId);
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(commonArea));
        List<CommonAreaReservation> commonAreaReservationList = [];
        CommonAreaReservation commonAreaReservation = new()
        {
            Id = _faker.Random.Int(),
            CommonAreaId = commonArea.Id,
            Date = DateOnly.FromDateTime(today),
            StartAt = TimeZoneHelper
                .ConvertFromUserTimeToUTC(reservationTime, timezoneOffsetMinutes),
            SlotId = slotId,
        };
        commonAreaReservationList.Add(commonAreaReservation);
        _commonAreaReservationsRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, It.IsAny<DateOnly>(), slotId))
            .Returns(Task.FromResult(commonAreaReservationList));
        CreateReservationCommand data = new()
        {
            Date = DateOnly.FromDateTime(today),
            StartAt = reservationTime,
            SlotId = slotId,
            TimezoneOffsetMinutes = timezoneOffsetMinutes
        };

        // Act
        var (result, reservationId) = await _commonAreasService.CreateReservationAsync(commonArea.Id, data);

        // Assert
        Assert.Equal(CreateReservationResult.UnavailableTimeSlot, result);
        Assert.Null(reservationId);
    }

    [Fact]
    public async Task Create_Reservation_Returns_Created()
    {
        // Arrange
        int timezoneOffsetMinutes = -180;
        var condominiumId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        commonArea.StartTime = new TimeOnly(6, 0);
        commonArea.EndTime = new TimeOnly(22, 0);
        commonArea.TimeInterval = new TimeOnly(1, 0);
        var slotId = commonArea.Slots.First().Id;
        var today = TimeZoneHelper.GetUserDateTime(timezoneOffsetMinutes);
        var reservationTime = new TimeOnly(today.AddHours(1).Hour, 0);
        _currentUserContextMock
            .Setup(mock => mock.GetIdentity())
            .Returns(userId);
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(commonArea));
        CreateReservationCommand data = new()
        {
            Date = DateOnly.FromDateTime(today),
            StartAt = reservationTime,
            SlotId = slotId,
            TimezoneOffsetMinutes = timezoneOffsetMinutes
        };

        // Act
        var (result, reservationId) = await _commonAreasService.CreateReservationAsync(commonArea.Id, data);

        // Assert
        Assert.Equal(CreateReservationResult.Created, result);
        _commonAreaReservationsRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<CommonAreaReservation>()), Times.Once);
        Assert.NotNull(reservationId);
    }
}