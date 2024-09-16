using MimeKit;
using NextCondoApi.Services.SMTP;

namespace UnitTests.Fakes;

public class SentEmailDetails
{
    public required string Code { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
}

public class FakeSMTPService : ISMTPService
{
    private List<SentEmailDetails> sentEmailDetails = [];

    public bool SendEmailVerification(string code, string name, string email)
    {
        sentEmailDetails.Add(new() { Code = code, Email = email, Name = name });
        return true;
    }

    public bool SendMessage(MimeMessage message)
    {
        return true;
    }

    public SentEmailDetails? GetByEmail(string email)
    {
        return sentEmailDetails.Find(emailDetails => emailDetails.Email.Equals(email));
    }
}
