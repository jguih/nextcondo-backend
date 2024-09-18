﻿using MimeKit;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using NextCondoApi.Services.SMTP;
using System.Security.Cryptography;

namespace NextCondoApi.Features.AuthFeature.Services;

public interface IEmailVerificationService
{
    /// <summary>
    /// Verify the validity of the code generated by CreateCode sent to the user.
    /// </summary>
    /// <param name="userId">Users's Id</param>
    /// <param name="email">User's email address</param>
    /// <param name="code">Code created by CreateCode</param>
    /// <returns>True if code is valid, false otherwise</returns>
    public Task<bool> VerifyCodeAsync(Guid userId, string email, string code);
    /// <summary>
    /// Creates a verification code that can be sent to the user by email.
    /// Code is a 8 lenght string of alphanumeric digits.
    /// </summary>
    /// <param name="userId">The user's Id</param>
    /// <param name="email">The user's email address</param>
    /// <returns></returns>
    public Task<string> CreateCodeAsync(Guid userId, string email);
    public bool IsCodeExpired(EmailVerificationCode emailCode);
    public void SendVerificationEmail(string code, string name, string email);
}

public class EmailVerificationService : IEmailVerificationService
{
    private readonly IEmailVerificationCodeRepository _emailVerificationCodeRepository;
    private readonly ISMTPService _smtpService;

    public EmailVerificationService(
        IEmailVerificationCodeRepository emailVerificationCodeRepository,
        ISMTPService smtpService)
    {
        _emailVerificationCodeRepository = emailVerificationCodeRepository;
        _smtpService = smtpService;
    }

    public async Task<bool> VerifyCodeAsync(Guid userId, string email, string code)
    {
        var existingCode = await _emailVerificationCodeRepository.GetEmailCodeForUser(userId);

        if (existingCode is null || !existingCode.Code.Equals(code))
        {
            return false;
        }

        await _emailVerificationCodeRepository.DeleteAsync(existingCode.Id);

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

    public async Task<string> CreateCodeAsync(Guid userId, string email)
    {
        await _emailVerificationCodeRepository.DeleteEmailCodesForUserAsync(userId);
        var code = RandomNumberGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVXWYZ0123456789", 8);
        var newEmailCode = new EmailVerificationCode()
        {
            UserId = userId,
            Email = email,
            Code = code,
            ExpirestAt = DateTime.UtcNow.AddMinutes(15)
        };
        await _emailVerificationCodeRepository.AddAsync(newEmailCode);
        return code;
    }

    public bool IsCodeExpired(EmailVerificationCode emailCode)
    {
        return DateTimeOffset.UtcNow.CompareTo(emailCode.ExpirestAt) > 0;
    }

    public void SendVerificationEmail(string code, string name, string email)
    {
        MimeMessage message = new();
        message.To.Add(new MailboxAddress(name, email));
        message.Subject = "Email verification";
        message.Body = new TextPart("plain") { Text = $"Use this code to verify your email address: {code}" };
        _smtpService.SendMessage(message);
    }
}
