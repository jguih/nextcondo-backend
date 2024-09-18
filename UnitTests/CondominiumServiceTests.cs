using Bogus;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Models.DTO;

namespace UnitTests;

public class CondominiumServiceTests
{
    private readonly CondominiumService _condominiumService;
    private readonly Mock<ICondominiumsRepository> _condominiumsRepository;
    private readonly Mock<ICurrentCondominiumRepository> _currentCondominiumRepository;
    private readonly Faker _faker;

    public CondominiumServiceTests()
    {
        _condominiumsRepository = new Mock<ICondominiumsRepository>();
        _currentCondominiumRepository = new Mock<ICurrentCondominiumRepository>();
        _faker = new Faker();
        _condominiumService = new CondominiumService(
            _currentCondominiumRepository.Object,
            _condominiumsRepository.Object);
    }

    [Fact]
    public async Task ReturnsCurrent_WhenItExists()
    {
        // Arrange
        var dtoMock = new Mock<CondominiumDTO>();
        _currentCondominiumRepository
            .Setup(mock => mock.GetDtoByUserIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(dtoMock.Object)!);

        // Act
        var result = await _condominiumService.GetCurrentAsync(_faker.Random.Guid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dtoMock.Object, result);
    }

    [Fact]
    public async Task ReturnsNull_WhenUserHasNoCondominiums()
    {
        // Arrange
        CondominiumDTO? condoDto = null;
        Guid? firstCondoId = null;
        var userId = _faker.Random.Guid();
        _currentCondominiumRepository
            .Setup(mock => mock.GetDtoByUserIdAsync(userId))
            .Returns(Task.FromResult(condoDto));
        _condominiumsRepository
            .Setup(mock => mock.GetFirstCondominiumIdForUser(userId))
            .Returns(Task.FromResult(firstCondoId));

        // Act
        var result = await _condominiumService.GetCurrentAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCurrent_WhenUserHasCondominiumButNoCurrent()
    {
        // Arrange
        var condoDtoMock = new Mock<CondominiumDTO>();
        CondominiumDTO? condoDto = null;
        Guid? firstCondoId = _faker.Random.Guid();
        var userId = _faker.Random.Guid();
        _currentCondominiumRepository
            .SetupSequence(mock => mock.GetDtoByUserIdAsync(userId))
            .Returns(Task.FromResult(condoDto))
            .Returns(Task.FromResult(condoDtoMock.Object)!);
        _condominiumsRepository
            .Setup(mock => mock.GetFirstCondominiumIdForUser(userId))
            .Returns(Task.FromResult(firstCondoId));

        // Act
        var result = await _condominiumService.GetCurrentAsync(userId);

        // Assert
        _currentCondominiumRepository
            .Verify(mock => mock.AddAsync(It.IsAny<CurrentCondominium>()), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(condoDtoMock.Object, result);
    }
}
