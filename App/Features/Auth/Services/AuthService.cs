using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using NextCondoApi.Entity;
using NextCondoApi.Features.UsersFeature.Services;
using NextCondoApi.Services;
using System.Security.Claims;

namespace NextCondoApi.Features.AuthFeature.Services;

public interface IAuthService
{
    public Task<bool> LoginAsync(string email, string password, string scheme);
    public Task<bool> RegisterAsync(string fullName, string email, string password, string? phone, string scheme);
    public Task LogoutAsync(string scheme);
    public Task SendPasswordResetEmail(User user);
    public Task<bool> VerifyEmailVerificationCodeAsync(string code, Guid userId);
    public Task SendEmailVerificationCodeAsync(Guid userId, string fullName, string email);
}

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly IAuthServiceHelper _helper;
    private readonly IEmailVerificationService _emailVerificationService;

    public AuthService(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IPasswordHasher<User> hasher,
        IDataProtectionProvider dataProtectionProvider,
        IAuthServiceHelper helper,
        IEmailVerificationService emailVerificationService)
    {
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _hasher = hasher;
        _dataProtectionProvider = dataProtectionProvider;
        _helper = helper;
        _emailVerificationService = emailVerificationService;
    }

    public async Task<bool> RegisterAsync(
        string fullName,
        string email,
        string password,
        string? phone,
        string scheme)
    {
        var existing = await _usersRepository.GetByEmailAsync(email);

        if (existing != null)
        {
            if (!existing.IsEmailVerified)
            {
                await SendEmailVerificationCodeAsync(existing.Id, fullName, email);
            }
            return true;
        }

        Role defaultRole = await _rolesRepository.GetDefaultAsync();
        var user = new User()
        {
            Email = email,
            FullName = fullName,
            Phone = phone,
            RoleId = defaultRole.Name,
        };
        var passwordHash = _hasher.HashPassword(user, password);
        user.PasswordHash = passwordHash;
        await _usersRepository.AddAsync(user);
        await SendEmailVerificationCodeAsync(user.Id, fullName, email);

        return true;
    }

    public async Task<bool> LoginAsync(string email, string password, string scheme)
    {
        var user = await _usersRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return false;
        }
        var claims = user.GetClaims();
        var identity = new ClaimsIdentity(claims, scheme);
        var principal = new ClaimsPrincipal(identity);
        await _helper.SignInAsync(scheme, principal);
        return true;
    }

    public async Task SendEmailVerificationCodeAsync(Guid userId, string fullName, string email)
    {
        var code = await _emailVerificationService.CreateCodeAsync(userId, email);
        _emailVerificationService.SendVerificationEmail(code, fullName, email);
    }

    public async Task<bool> VerifyEmailVerificationCodeAsync(string code, Guid userId)
    {
        var user = await _usersRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return false;
        }

        var result = await _emailVerificationService.VerifyCodeAsync(user.Id, user.Email, code);

        if (result == true)
        {
            user.IsEmailVerified = true;
            user.EmailVerifiedAt = DateTimeOffset.UtcNow;
            await _usersRepository.UpdateAsync(user);
        }

        return result;
    }

    public Task SendPasswordResetEmail(User user)
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync(string scheme)
    {
        await _helper.SignOutAsync(scheme);
    }
}
