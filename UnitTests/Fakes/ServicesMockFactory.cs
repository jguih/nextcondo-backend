

using Bogus;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Moq;
using NextCondoApi.Entity;

namespace UnitTests.Fakes;

public class ServicesMockFactory
{
    private readonly Faker Faker;

    public ServicesMockFactory()
    {
        Faker = new Faker();
    }

    public Mock<IPasswordHasher<User>> GetUserPasswordHasher()
    {
        var hasher = new Mock<IPasswordHasher<User>>();

        hasher
            .Setup(mock => mock.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(Faker.Random.Hash());

        hasher
            .Setup(mock => mock.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);

        return hasher;
    }

    public Mock<IDataProtectionProvider> GetDataProtectionProvider()
    {
        var provider = new Mock<IDataProtectionProvider>();
        return provider;
    }
}
