using MimeKit;

namespace NextCondoApi.Services.SMTP;

public interface ISMTPService
{
    public void SendMessage(MimeMessage message);
}
