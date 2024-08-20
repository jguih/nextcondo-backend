
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Moq;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using NextCondoApi.Services.Auth;
using System.Security.Claims;

namespace UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUsersRepository> usersRepository;
    private readonly Mock<IRolesRepository> rolesRepository;
    private readonly Mock<IPasswordHasher<User>> hasher;
    private readonly Mock<IDataProtectionProvider> dataProtectionProvider;
    private readonly IAuthServiceHelper authServiceHelper;
    private readonly IAuthService authService;

    public AuthServiceTests()
    {
        usersRepository = new Mock<IUsersRepository>();
        rolesRepository = new Mock<IRolesRepository>();
        hasher = new Mock<IPasswordHasher<User>>();
        dataProtectionProvider = new Mock<IDataProtectionProvider>();
        authServiceHelper = new FakeAuthServiceHelper();

        hasher
            .Setup(mock => mock.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns("!$%$!@#111231$!@312");

        Role mockedRole = new() { Name = "Tenant" };
        rolesRepository
            .Setup(mock => mock.GetDefaultAsync())
            .Returns(Task.FromResult(mockedRole));

        authService = new AuthService(
                usersRepository.Object,
                rolesRepository.Object,
                hasher.Object,
                dataProtectionProvider.Object,
                authServiceHelper
            );
    }

    [Fact]
    public async void Test1()
    {
        usersRepository
            .Setup(mock => mock.GetByEmailAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<User?>(null));

        usersRepository
            .Setup(mock => mock.AddAsync(It.IsAny<User>()))
            .Returns(Task.FromResult(true));

        usersRepository
            .Setup(mock => mock.SaveAsync())
            .Returns(Task.FromResult(1));

        var result = await authService.RegisterAsync(
                fullName: "Testing",
                email: "email@email.com",
                password: "password",
                phone: "55 1234556",
                scheme: "local"
            );

        Assert.True(result);
    }

    [Fact]
    public async void ReturnsTrue_WhenProvidedEmailExists()
    {
        var mockUser = new Mock<User>();
        var userEmail = "email@email.com";
        usersRepository
            .Setup(mock => mock.GetByEmailAsync(userEmail))
            .Returns(Task.FromResult<User?>(mockUser.Object));

        var result = await authService.RegisterAsync(
                fullName: "Testing",
                email: userEmail,
                password: "password",
                phone: "55 1234556",
                scheme: "local"
            );

        usersRepository
            .Verify(repo => repo.GetByEmailAsync(userEmail), Times.Once);
        rolesRepository
            .Verify(repo => repo.GetDefaultAsync(), Times.Never);
        Assert.True(result);
    }
}