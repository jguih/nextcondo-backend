
using Bogus;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Features.AuthFeature.Services;
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
    private Mock<IEmailVerificationService> EmailVerificationServiceMock { get; set; } = null!;
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
        EmailVerificationServiceMock = new Mock<IEmailVerificationService>();

        AuthService = new AuthService(
                UsersRepository,
                RolesRepository,
                Hasher.Object,
                DataProtectionProvider.Object,
                AuthServiceHelper,
                EmailVerificationServiceMock.Object
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
        var all = await UsersRepository.GetAllAsync();

        // Assert
        Assert.Single(all);
        Assert.True(result);
    }

    [Fact]
    public async Task LoginUser_IfUserExists()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();

        // Act
        var registerResult = await AuthService.RegisterAsync(
                fullName: userDetails.FullName,
                email: userDetails.Email,
                password: userDetails.Password,
                phone: userDetails.Phone,
                scheme: "local"
            );
        var loginResult = await AuthService
            .LoginAsync(
                email: userDetails.Email,
                password: userDetails.Password,
                scheme: "local"
            );

        // Assert
        Assert.True(registerResult);
        Assert.True(loginResult);
    }

    [Fact]
    public async Task DoesNotLogin_IfUserNotRegistered()
    {
        // Arrange
        var userDetails = FakeUsersFactory.GetFakeUserDetails();

        // Act
        var loginResult = await AuthService
            .LoginAsync(
                email: userDetails.Email,
                password: userDetails.Password,
                scheme: "local"
            );

        // Assert
        Assert.False(loginResult);
    }

    [Fact]
    public async Task SendVerificationEmail()
    {
        // Arrange
        var user = FakeUsersFactory.GetFakeUser();
        var code = Faker.Random.AlphaNumeric(8);
        EmailVerificationServiceMock
            .Setup(m => m.CreateCodeAsync(user.Id, user.Email))
            .Returns(Task.FromResult(code));

        // Act
        await AuthService.SendEmailVerificationCodeAsync(user.Id, user.FullName, user.Email);

        // Assert
        EmailVerificationServiceMock
            .Verify(m => m.SendVerificationEmail(
                It.Is<string>(ms => ms.Equals(code)),
                It.Is<string>(ms => ms.Equals(user.FullName)),
                It.Is<string>(ms => ms.Equals(user.Email))
            ));
    }

    [Fact]
    public async Task EmailVerification_FailsIfNoCodeWasSentForUser()
    {
        // Arrange
        var user = FakeUsersFactory.GetFakeUser();
        var code = Faker.Random.AlphaNumeric(8);

        // Act
        var result = await AuthService.VerifyEmailVerificationCodeAsync(code, user.Id);

        // Assert
        Assert.False(result);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}