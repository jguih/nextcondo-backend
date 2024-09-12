using Bogus;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Controllers;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using TestFakes;
using UnitTests.Fakes;

namespace UnitTests;

public class CondominiumControllerTests
{
    private readonly CondominiumController _controller;
    private readonly FakeUsersRepository _usersRepository;
    private readonly FakeCondominiumsRepository _condominiumsRepository;
    private readonly Faker _faker;

    public CondominiumControllerTests()
    {
        _condominiumsRepository = new FakeCondominiumsRepository();
        _usersRepository = new FakeUsersRepository();
        _controller = new CondominiumController(_condominiumsRepository, _usersRepository);
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public async Task FailToAdd_WhenProvidedOwnerDoesNotExist()
    {
        // Arrange
        AddCondominiumDTO newCondo = new()
        {
            Name = _faker.Company.CompanyName(),
            Description = _faker.Lorem.Paragraph(3),
            OwnerId = _faker.Random.Guid(),
            RelationshipType = CondominiumUserRelationshipType.Tenant,
        };

        // Act
        var result = await _controller.AddAsync(newCondo);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var details = (result as ObjectResult)?.Value;
        Assert.IsType<ProblemDetails>(details);
    }

    [Fact]
    public async Task Add_WhenProvidedOwnerExists()
    {
        // Arrange
        var fakeUser = FakeUsersFactory.GetFakeUser();
        await _usersRepository.AddAsync(fakeUser);
        AddCondominiumDTO newCondo = new()
        {
            Name = _faker.Company.CompanyName(),
            Description = _faker.Lorem.Paragraph(120),
            OwnerId = fakeUser.Id,
            RelationshipType = CondominiumUserRelationshipType.Tenant,
        };

        // Act
        var result = await _controller.AddAsync(newCondo);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        Assert.Single(await _condominiumsRepository.GetAllAsync());
    }
}
