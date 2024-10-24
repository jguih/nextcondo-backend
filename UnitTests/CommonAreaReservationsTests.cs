
using Bogus;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Features.CommonAreasFeature.Services;
using NextCondoApi.Services;
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
        var (result, bookingSlots) = await _commonAreasService.GetBookingSlotsAsync(commonAreaId, slotId);

        // Assert
        Assert.Equal(GetBookingSlotsResult.CommonAreaNotFound, result);
        Assert.Null(bookingSlots);
    }

    [Fact]
    public async Task GetTimeSlots_Returns_TimeSlotsArray()
    {
        // Arrange
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
        var (result, bookingSlots) = await _commonAreasService.GetBookingSlotsAsync(commonArea.Id, slotId);
        var first = bookingSlots?.First();

        // Assert
        Assert.Equal(GetBookingSlotsResult.Ok, result);
        Assert.NotNull(bookingSlots);
        Assert.NotEmpty(bookingSlots);
        Assert.Equal(7, bookingSlots.Count);
        Assert.NotNull(first);
        Assert.Equal(22, first.Slots.Count);
    }

    [Theory]
    [InlineData("06:00", "01:00")]
    [InlineData("01:30", "01:30")]
    [InlineData("03:00", "01:30")]
    [InlineData("20:00", "02:00")]
    [InlineData("16:00", "02:00")]
    public async Task GetTimeSlots_Returns_TimeSlotsArray_With_UnavailableSlot(string reservationTime, string interval)
    {
        // Arrange
        var condominiumId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        commonArea.TimeInterval = TimeOnly.Parse(interval);
        var slotId = commonArea.Slots.First().Id;
        var now = DateTime.UtcNow;
        var today = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
        List<CommonAreaReservation> commonAreaReservationList = [];
        CommonAreaReservation commonAreaReservation = new()
        {
            Id = _faker.Random.Int(),
            CommonAreaId = commonArea.Id,
            Date = today,
            StartAt = TimeOnly.Parse(reservationTime),
            SlotId = slotId,
        };
        commonAreaReservationList.Add(commonAreaReservation);
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonArea?>(commonArea));
        _commonAreaReservationsRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, It.IsAny<DateOnly>(), slotId))
            .Returns(Task.FromResult(commonAreaReservationList));

        // Act
        var (result, bookingSlots) = await _commonAreasService.GetBookingSlotsAsync(commonArea.Id, slotId);
        var timeSlotsForToday = bookingSlots?.Find(timeSlot => timeSlot.Date.CompareTo(today) == 0);
        var unavailableTimeSlot = timeSlotsForToday?.Slots
            .Find(slot => slot.StartAt.CompareTo(commonAreaReservation.StartAt) == 0);

        // Assert
        Assert.Equal(GetBookingSlotsResult.Ok, result);
        Assert.NotNull(unavailableTimeSlot);
        Assert.False(unavailableTimeSlot.Available);
    }

    [Fact]
    public async Task Create_Reservation_Returns_CommonAreaNotFound()
    {
        // Arrange
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
            SlotId = slotId
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
        var condominiumId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        var slotId = commonArea.Slots.First().Id;
        var now = DateTime.UtcNow;
        var today = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
        var reservationTime = TimeOnly.Parse("14:00");
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
            Date = today,
            StartAt = reservationTime,
            SlotId = slotId,
        };
        commonAreaReservationList.Add(commonAreaReservation);
        _commonAreaReservationsRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id, It.IsAny<DateOnly>(), slotId))
            .Returns(Task.FromResult(commonAreaReservationList));
        CreateReservationCommand data = new()
        {
            Date = today,
            StartAt = reservationTime,
            SlotId = slotId
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
        var condominiumId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        CommonArea commonArea = FakeCommonAreasFactory.Get();
        var slotId = commonArea.Slots.First().Id;
        var now = DateTime.UtcNow;
        var today = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
        var reservationTime = TimeOnly.Parse("14:00");
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
            StartAt = reservationTime,
            SlotId = slotId,
        };

        // Act
        var (result, reservationId) = await _commonAreasService.CreateReservationAsync(commonArea.Id, data);

        // Assert
        Assert.Equal(CreateReservationResult.Created, result);
        _commonAreaReservationsRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<CommonAreaReservation>()), Times.Once);
        Assert.NotNull(reservationId);
    }
}