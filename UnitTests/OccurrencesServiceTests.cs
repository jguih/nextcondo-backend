
using Bogus;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Features.OccurrencesFeature.Models;
using NextCondoApi.Features.OccurrencesFeature.Services;
using TestFakes;

namespace UnitTests;

public class OccurrencesServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<ICurrentCondominiumRepository> _currentCondominiumRepositoryMock;
    private readonly Mock<IOccurrencesRepository> _occurrencesRepositoryMock;
    private readonly Mock<IOccurrenceTypesRepository> _occurrenceTypesRepositoryMock;
    private readonly OccurrencesService _occurrencesService;

    public OccurrencesServiceTests()
    {
        _faker = new Faker();
        _currentCondominiumRepositoryMock = new Mock<ICurrentCondominiumRepository>();
        _occurrencesRepositoryMock = new Mock<IOccurrencesRepository>();
        _occurrenceTypesRepositoryMock = new Mock<IOccurrenceTypesRepository>();
        _occurrencesService = new(
            _occurrencesRepositoryMock.Object,
            _currentCondominiumRepositoryMock.Object,
            _occurrenceTypesRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Add_Returns2_Guid_Null()
    {
        // Arrange
        var data = FakeOccurrencesFactory.GetFakeNewOccurrenceDetails();
        var userId = _faker.Random.Guid();
        _currentCondominiumRepositoryMock
            .Setup(mock => mock.GetCondominiumIdAsync(userId))
            .Returns(Task.FromResult<Guid?>(null));

        // Act
        var result = await _occurrencesService.AddAsync(data, userId);

        // Assert
        Assert.Equal(2, result.result);
    }

    [Fact]
    public async Task Add_Returns2_Guid_Default()
    {
        // Arrange
        var data = FakeOccurrencesFactory.GetFakeNewOccurrenceDetails();
        var userId = _faker.Random.Guid();
        _currentCondominiumRepositoryMock
            .Setup(mock => mock.GetCondominiumIdAsync(userId))
            .Returns(Task.FromResult<Guid?>(default));

        // Act
        var result = await _occurrencesService.AddAsync(data, userId);

        // Assert
        Assert.Equal(2, result.result);
    }

    [Fact]
    public async Task Add_Returns1()
    {
        // Arrange
        var data = FakeOccurrencesFactory.GetFakeNewOccurrenceDetails();
        var userId = _faker.Random.Guid();
        var condoId = _faker.Random.Guid();
        _currentCondominiumRepositoryMock
            .Setup(mock => mock.GetCondominiumIdAsync(userId))
            .Returns(Task.FromResult<Guid?>(condoId));
        _occurrenceTypesRepositoryMock
            .Setup(mock => mock.GetByIdAsync(data.OccurrenceTypeId))
            .Returns(Task.FromResult<OccurrenceType?>(null));

        // Act
        var result = await _occurrencesService.AddAsync(data, userId);

        // Assert
        Assert.Equal(1, result.result);
    }
}