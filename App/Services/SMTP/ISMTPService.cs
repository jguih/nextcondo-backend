using MimeKit;

namespace NextCondoApi.Services.SMTP;

public interface ISMTPService
{
    public bool SendMessage(MimeMessage message);
    public bool SendEmailVerification(string code, string name, string email);
}
