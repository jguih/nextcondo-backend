using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using NextCondoApi.Entity;
using NextCondoApi.Services.SMTP;
using System.Security.Claims;

namespace NextCondoApi.Services.Auth;

public interface IAuthService
{
    public Task<bool> LoginAsync(string email, string password, string scheme);
    public Task<bool> RegisterAsync(string fullName, string email, string password, string? phone, string scheme);
    public Task LogoutAsync(string scheme);
    public Task SendPasswordResetEmail(User user);
    public Task<bool> VerifyEmailVerificationCodeAsync(string code, Guid userId);
    public Task<bool> SendEmailVerificationCodeAsync(Guid userId, string fullName, string email);
}

public class AuthService : IAuthService
{
    private readonly IUsersRepository usersRepository;
    private readonly IRolesRepository rolesRepository;
    private readonly IPasswordHasher<User> hasher;
    private readonly IDataProtectionProvider dataProtectionProvider;
    private readonly IAuthServiceHelper helper;
    private readonly ISMTPService smtpService;
    private readonly IEmailVerificationCodeRepository emailVerificationCodeRepository;

    public AuthService(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IPasswordHasher<User> hasher,
        IDataProtectionProvider dataProtectionProvider,
        IAuthServiceHelper helper,
        ISMTPService smtpService,
        IEmailVerificationCodeRepository emailVerificationCodeRepository)
    {
        this.usersRepository = usersRepository;
        this.rolesRepository = rolesRepository;
        this.hasher = hasher;
        this.dataProtectionProvider = dataProtectionProvider;
        this.helper = helper;
        this.smtpService = smtpService;
        this.emailVerificationCodeRepository = emailVerificationCodeRepository;
    }

    public async Task<bool> RegisterAsync(
        string fullName,
        string email,
        string password,
        string? phone,
        string scheme)
    {
        var existing = await usersRepository.GetByEmailAsync(email);

        if (existing != null)
        {
            if (!existing.IsEmailVerified)
            {
                await SendEmailVerificationCodeAsync(existing.Id, fullName, email);
            }
            return true;
        }

        Role defaultRole = await rolesRepository.GetDefaultAsync();
        var user = new User()
        {
            Email = email,
            FullName = fullName,
            Phone = phone,
            Role = defaultRole,
            RoleId = defaultRole.Name,
        };
        var passwordHash = hasher.HashPassword(user, password);
        user.PasswordHash = passwordHash;
        await usersRepository.AddAsync(user);
        await SendEmailVerificationCodeAsync(user.Id, fullName, email);

        return true;
    }

    public async Task<bool> LoginAsync(string email, string password, string scheme)
    {
        var user = await usersRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return false;
        }
        var claims = user.GetClaims();
        var identity = new ClaimsIdentity(claims, scheme);
        var principal = new ClaimsPrincipal(identity);
        await helper.SignInAsync(scheme, principal);
        return true;
    }

    public async Task<bool> SendEmailVerificationCodeAsync(Guid userId, string fullName, string email)
    {
        var code = await emailVerificationCodeRepository.CreateCodeAsync(userId, email);
        return smtpService.SendEmailVerification(code, fullName, email);
    }

    public async Task<bool> VerifyEmailVerificationCodeAsync(string code, Guid userId)
    {
        var user = await usersRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return false;
        }

        var result = await emailVerificationCodeRepository.VerifyCodeAsync(user.Id, user.Email, code);

        if (result == true)
        {
            user.IsEmailVerified = true;
            user.EmailVerifiedAt = DateTimeOffset.UtcNow;
            await usersRepository.UpdateAsync(user);
        }

        return result;
    }

    public Task SendPasswordResetEmail(User user)
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync(string scheme)
    {
        await helper.SignOutAsync(scheme);
    }
}
