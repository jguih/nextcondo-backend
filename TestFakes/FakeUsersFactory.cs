using Bogus;
using NextCondoApi.Entity;
using NextCondoApi.Features.AuthFeature.Models;

namespace TestFakes;

public class RegisterUserDetails : RegisterUserDTO;

public static class FakeUsersFactory
{
    private static Faker<User> UserFaker { get; } = new Faker<User>()
        .RuleFor(o => o.Id, f => f.Random.Guid())
        .RuleFor(o => o.Email, f => f.Internet.Email())
        .RuleFor(o => o.RoleId, f => FakeRolesFactory.GetDefault().Name)
        .RuleFor(o => o.CreatedAt, f => DateTimeOffset.UtcNow)
        .RuleFor(o => o.UpdatedAt, f => DateTimeOffset.UtcNow)
        .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
        .RuleFor(o => o.FullName, f => f.Name.FullName())
        .RuleFor(o => o.PasswordHash, f => f.Random.Hash())
        .RuleFor(o => o.IsEmailVerified, true)
        .RuleFor(o => o.EmailVerifiedAt, DateTimeOffset.UtcNow);

    private static Faker<RegisterUserDetails> RegisterUserDetailsFaker { get; } = new Faker<RegisterUserDetails>()
        .RuleFor(o => o.Email, f => f.Internet.Email())
        .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber())
        .RuleFor(o => o.FullName, f => f.Name.FullName())
        .RuleFor(o => o.Password, f => f.Internet.Password());

    public static User GetFakeUser()
    {
        return UserFaker.Generate();
    }

    public static RegisterUserDetails GetFakeUserDetails()
    {
        return RegisterUserDetailsFaker.Generate();
    }
}
