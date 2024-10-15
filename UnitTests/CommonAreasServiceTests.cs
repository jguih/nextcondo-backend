
using Bogus;
using Bogus.Extensions;
using Moq;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Features.CommonAreasFeature.Services;
using NextCondoApi.Services;

namespace UnitTests;

public class CommonAreasServiceTests
{
    private readonly CommonAreasService _commonAreasService;
    private readonly Mock<ICurrentUserContext> _currentUserContextMock;
    private readonly Mock<ICommonAreasRepository> _commonAreasRepositoryMock;
    private readonly Faker _faker;

    public CommonAreasServiceTests()
    {
        _currentUserContextMock = new Mock<ICurrentUserContext>();
        _commonAreasRepositoryMock = new Mock<ICommonAreasRepository>();
        _faker = new Faker();
        _commonAreasService = new(
            _commonAreasRepositoryMock.Object,
            _currentUserContextMock.Object);
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
        CommonAreaDTO commonArea = new()
        {
            Id = _faker.Random.Int(),
            Name = _faker.Lorem.Sentence(3).ClampLength(0, 255),
            Description = _faker.Lorem.Paragraph(10).ClampLength(0, 2000),
            StartTime = TimeOnly.Parse("00:00"),
            EndTime = TimeOnly.Parse("22:00"),
            TimeInterval = TimeOnly.Parse("01:00")
        };
        _currentUserContextMock
            .Setup(mock => mock.GetCurrentCondominiumIdAsync())
            .Returns(Task.FromResult(condominiumId));
        _commonAreasRepositoryMock
            .Setup(mock => mock.GetDtoAsync(commonArea.Id, condominiumId))
            .Returns(Task.FromResult<CommonAreaDTO?>(commonArea));

        // Act
        var result = await _commonAreasService.GetTimeSlotsAsync(commonArea.Id);
        var first = result?.First();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(7, result.Count);
        Assert.NotNull(first);
        Assert.Equal(23, first.Slots.Count);
    }
}