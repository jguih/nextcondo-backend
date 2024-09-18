using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NextCondoApi.Features.Configuration;

namespace NextCondoApi.Services.SMTP;

public class SMTPService : ISMTPService
{
    private readonly IOptions<SMTPOptions> _smtpConfig;
    private readonly string host;
    private readonly int port;
    private readonly string? username;
    private readonly string? password;
    private readonly string defaultFrom;

    public SMTPService(IOptions<SMTPOptions> smtpConfig)
    {
        _smtpConfig = smtpConfig;
        host = _smtpConfig.Value.HOST;
        port = _smtpConfig.Value.PORT;
        username = _smtpConfig.Value.USERNAME;
        password = _smtpConfig.Value.PASSWORD;
        defaultFrom = _smtpConfig.Value.DEFAULT_FROM;
    }

    public void SendMessage(MimeMessage message)
    {
        message.From.Add(new MailboxAddress("NextCondo", defaultFrom));

        using (var client = new SmtpClient())
        {
            client.Connect(host, port, false);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                client.Authenticate(username, password);
            }

            client.Send(message);
            client.Disconnect(true);
        }
    }
}
