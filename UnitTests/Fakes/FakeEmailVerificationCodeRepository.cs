using Bogus;
using NextCondoApi.Entity;
using NextCondoApi.Services;

namespace UnitTests.Fakes;

public class FakeEmailVerificationCodeRepository : 
    FakeGenericRepository<EmailVerificationCode>, 
    IEmailVerificationCodeRepository
{
    private readonly Faker _faker;

    public FakeEmailVerificationCodeRepository()
    {
        _faker = new Faker("pt_BR");
    }

    public async Task<string> CreateCodeAsync(Guid userId, string email)
    {
        await Task.Delay(1);
        var existing = Entities
            .Find(emailCode => emailCode.UserId.Equals(userId));
        if (existing != null)
        {
            Entities.Remove(existing);
        }
        var code = _faker.Random.AlphaNumeric(8);
        Entities.Add(new EmailVerificationCode()
        {
            Id = _faker.Random.Number(),
            UserId = userId,
            Email = email,
            Code = code,
            ExpirestAt = DateTimeOffset.UtcNow.AddMinutes(30),
        });
        return code;
    }

    public override async Task<EmailVerificationCode?> GetByIdAsync(object id)
    {
        await Task.Delay(1);
        var emailCode = Entities.Find(e => e.Id.Equals(id));
        return emailCode;
    }

    public async Task<bool> VerifyCodeAsync(Guid userId, string email, string code)
    {
        await Task.Delay(1);
        var existingCode = Entities.Find(emailCode => emailCode.UserId.Equals(userId));

        if (existingCode == null || !existingCode.Code.Equals(code))
        {
            return false;
        }

        Entities.Remove(existingCode);

        if (IsCodeExpired(existingCode))
        {
            return false;
        }

        if (!existingCode.Email.Equals(email))
        {
            return false;
        }

        return true;
    }

    public bool IsCodeExpired(EmailVerificationCode emailCode)
    {
        return DateTimeOffset.UtcNow.CompareTo(emailCode.ExpirestAt) > 0;
    }
}