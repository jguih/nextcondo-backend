
using TestFakes;
using UnitTests.Fakes;

namespace UnitTests;

public class RepositoryTests
{
    [Fact]
    public async Task FindUserById()
    {
        // Arrange
        var usersRepository = new FakeUsersRepository();
        var user = FakeUsersFactory.GetFakeUser();

        // Act
        await usersRepository.AddAsync(user);
        var created = await usersRepository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(created);
    }

    [Fact]
    public async Task FindRoleById()
    {
        // Arrange
        var rolesRepository = await FakeRolesRepository.Create();

        // Act
        var defaultRole = await rolesRepository.GetByIdAsync(FakeRolesFactory.GetDefault().Name);

        // Assert
        Assert.NotNull(defaultRole);
    }
}