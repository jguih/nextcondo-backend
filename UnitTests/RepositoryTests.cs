
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

    [Fact]
    public async Task CreateEmailVerificationCode()
    {
        // Arrange
        var usersRepository = new FakeUsersRepository();
        var user = FakeUsersFactory.GetFakeUser();
        await usersRepository.AddAsync(user);
        var emailVerificationCodeRepository = new FakeEmailVerificationCodeRepository();

        // Act
        var code = await emailVerificationCodeRepository.CreateCodeAsync(user.Id, user.Email);
        var all = await emailVerificationCodeRepository.GetAllAsync();
        var first = all.FirstOrDefault();
        var isExpired = first is null || emailVerificationCodeRepository.IsCodeExpired(first);

        // Assert
        Assert.NotNull(code);
        Assert.Single(all);
        Assert.NotNull(first);
        Assert.Equal(user.Email, first.Email);
        Assert.Equal(user.Id, first.UserId);
        Assert.Equal(code, first.Code);
        Assert.False(isExpired);
    }
}