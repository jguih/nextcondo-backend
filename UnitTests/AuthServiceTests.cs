
using Bogus;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Services.Auth;
using TestFakes;
using UnitTests.Fakes;

namespace UnitTests;

public class AuthServiceTests : IAsyncLifetime
{
    private FakeUsersRepository UsersRepository { get; set; } = null!;
    private FakeRolesRepository RolesRepository { get; set; } = null!;
    private FakeAuthServiceHelper AuthServiceHelper { get; set; } = null!;
    private Mock<IPasswordHasher<User>> Hasher { get; set; } = null!;
    private Mock<IDataProtectionProvider> DataProtectionProvider { get; set; } = null!;
    private AuthService AuthService { get; set; } = null!;
    private Faker Faker { get; set; }

    public AuthServiceTests()
    {
        Faker = new Faker("pt_BR");
    }

    private async Task ConfigureServices()
    {
        var servicesMockFactory = new ServicesMockFactory();
        RolesRepository = await FakeRolesRepository.Create();
        UsersRepository = new FakeUsersRepository();
        AuthServiceHelper = new FakeAuthServiceHelper();
        Hasher = servicesMockFactory.GetUserPasswordHasher();
        DataProtectionProvider = servicesMockFactory.GetDataProtectionProvider();

        AuthService = new AuthService(
                UsersRepository,
                RolesRepository,
                Hasher.Object,
                DataProtectionProvider.Object,
                AuthServiceHelper
            );
    }

    public async Task InitializeAsync()
    {
        await ConfigureServices();
    }

    [Fact]
    public async Task RegisterUser_WhenOneWithSameEmailDoesNotExist()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();

        // Act
        var result = await AuthService.RegisterAsync(
                fullName: userDetails.FullName,
                email: userDetails.Email,
                password: userDetails.Password,
                phone: userDetails.Phone,
                scheme: "local"
            );
        var created = await UsersRepository.GetByEmailAsync(userDetails.Email);

        // Assert
        Assert.True(result);
        Assert.NotNull(created);
    }

    [Fact]
    public async Task DoesNotRegisterUser_WhenProvidedEmailExists()
    {
        // Arrange
        var password = Faker.Internet.Password();
        var fakeUser = FakeUsersFactory.GetFakeUser();
        await UsersRepository.AddAsync(fakeUser);

        // Act
        var result = await AuthService.RegisterAsync(
                fullName: fakeUser.FullName,
                email: fakeUser.Email,
                password: password,
                phone: fakeUser.Phone,
                scheme: "local"
            );
        var allUsers = await UsersRepository.GetAllAsync();

        // Assert
        Assert.True(result);
        Assert.Single(allUsers);
    }

    [Fact]
    public async Task LoginUser_IfUserExists()
    {
        // Arrange
        User fakeUser = FakeUsersFactory.GetFakeUser();
        var password = Faker.Internet.Password();
        await UsersRepository.AddAsync(fakeUser);

        // Act
        var result = await AuthService
            .LoginAsync(
                email: fakeUser.Email,
                password,
                scheme: "local"
            );

        // Assert
        Assert.True(result);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}