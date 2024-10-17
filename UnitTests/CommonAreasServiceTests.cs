
using Bogus;
using Bogus.Extensions;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Features.CommonAreasFeature.Services;
using NextCondoApi.Services;
using TestFakes;

namespace UnitTests;

public class CommonAreasServiceTests
{
    private readonly CommonAreasService _commonAreasService;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<ICommonAreasRepository> _commonAreasRepositoryMock;
    private readonly Mock<ICommonAreaReservationsRepository> _commonAreaReservationsRepositoryMock;
    private readonly Faker _faker;

    public CommonAreasServiceTests()
    {
        _currentUserContextMock = new Mock<ICurrentUserContext>();
        _commonAreasRepositoryMock = new Mock<ICommonAreasRepository>();
        _commonAreaReservationsRepositoryMock = new Mock<ICommonAreaReservationsRepository>();
        _faker = new Faker();
        _commonAreasService = new(
            _commonAreasRepositoryMock.Object,
            _currentUserContextMock.Object,
            _commonAreaReservationsRepositoryMock.Object);
    }

    [Fact]
    public async Task GetTimeSlots_Returns_Null()
    {
        // Arrange
        var commonAreaId = _faker.Random.Int();
        var condominiumId = _faker.Random.Guid();
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetDtoAsync(commonAreaId, condominiumId))
            .Returns(Task.FromResult<CommonAreaDTO?>(null));

        // Act
        var result = await _commonAreasService.GetTimeSlotsAsync(commonAreaId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTimeSlots_Returns_TimeSlotsArray()
    {
        // Arrange
        var condominiumId = _faker.Random.Guid();
        CommonAreaDTO commonArea = FakeCommonAreasFactory.GetDto();
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetDtoAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonAreaDTO?>(commonArea));
        _commonAreaReservationsRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id))
            .Returns(Task.FromResult<List<CommonAreaReservation>>([]));

        // Act
        var result = await _commonAreasService.GetTimeSlotsAsync(commonArea.Id);
        var first = result?.First();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(7, result.Count);
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
        CommonAreaDTO commonArea = FakeCommonAreasFactory.GetDto();
        commonArea.TimeInterval = TimeOnly.Parse(interval);
        var now = DateTime.UtcNow;
        var today = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
        List<CommonAreaReservation> commonAreaReservationList = [];
        CommonAreaReservation commonAreaReservation = new()
        {
            Id = _faker.Random.Int(),
            CommonAreaId = commonArea.Id,
            Date = today,
            StartAt = TimeOnly.Parse(reservationTime)
        };
        commonAreaReservationList.Add(commonAreaReservation);
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetDtoAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonAreaDTO?>(commonArea));
        _commonAreaReservationsRepositoryMock
            .Setup(mock => mock.GetAsync(commonArea.Id))
            .Returns(Task.FromResult(commonAreaReservationList));

        // Act
        var result = await _commonAreasService.GetTimeSlotsAsync(commonArea.Id);
        var timeSlotsForToday = result?.Find(timeSlot => timeSlot.Date.CompareTo(today) == 0);
        var unavailableTimeSlot = timeSlotsForToday?.Slots
            .Find(slot => slot.StartAt.CompareTo(commonAreaReservation.StartAt) == 0);

        // Assert
        Assert.NotNull(unavailableTimeSlot);
        Assert.False(unavailableTimeSlot.Available);
    }
}