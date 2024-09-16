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

    public bool SendMessage(MimeMessage message)
    {
        using (var client = new SmtpClient())
        {
            client.Connect(host, port, false);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                client.Authenticate(username, password);
            }

            client.Send(message);
            client.Disconnect(true);

            return true;
        }
    }

    public bool SendEmailVerification(string code, string name, string email)
    {
        MimeMessage message = new();
        message.From.Add(new MailboxAddress("NextCondo", defaultFrom));
        message.To.Add(new MailboxAddress(name, email));
        message.Subject = "Email verification";
        message.Body = new TextPart("plain") { Text = $"Use this code to verify your email address: {code}" };

        return SendMessage(message);
    }
}
