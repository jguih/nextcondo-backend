
using MimeKit;
using NextCondoApi.Services.SMTP;

namespace IntegrationTests.Fakes;

public class FakeSMTPService : ISMTPService
{
    public void SendMessage(MimeMessage message)
    {
        return;
    }
}