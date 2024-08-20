using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using NextCondoApi.Entity;
using System.Security.Claims;

namespace NextCondoApi.Services.Auth;

public interface IAuthService
{
    public Task<bool> LoginAsync(string email, string password, string scheme);
    public Task<bool> RegisterAsync(string fullName, string email, string password, string? phone, string scheme);
    public Task SendPasswordResetEmail(string email);
    public Task LogoutAsync(string scheme);
}

public class AuthService : IAuthService
{
    private readonly IUsersRepository usersRepository;
    private readonly IRolesRepository rolesRepository;
    private readonly IPasswordHasher<User> hasher;
    private readonly IDataProtectionProvider dataProtectionProvider;
    private readonly IAuthServiceHelper helper;

    private static readonly bool LOGIN_AFTER_REGISTER = false;

    public AuthService(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IPasswordHasher<User> hasher,
        IDataProtectionProvider dataProtectionProvider,
        IAuthServiceHelper helper)
    {
        this.usersRepository = usersRepository;
        this.rolesRepository = rolesRepository;
        this.hasher = hasher;
        this.dataProtectionProvider = dataProtectionProvider;
        this.helper = helper;
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
            // TODO: Send verification email
            return true;
        }

        var user = new User()
        {
            Email = email,
            FullName = fullName,
            Phone = phone,
            RoleId = (await rolesRepository.GetDefaultAsync()).Name,
        };
        var passwordHash = hasher.HashPassword(user, password);
        user.PasswordHash = passwordHash;

        await usersRepository.AddAsync(user);
        await usersRepository.SaveAsync();

        // TODO: Send verification email

        if (LOGIN_AFTER_REGISTER == true)
        {
            var identity = new ClaimsIdentity(user.GetClaims(), scheme);
            var userPrincipal = new ClaimsPrincipal(identity);
            await helper.SignInAsync(scheme, userPrincipal);
        }

        return true;
    }

    public async Task<bool> LoginAsync(string email, string password, string scheme)
    {
        var user = await usersRepository.GetByEmailAsync(email);
        if (user != null)
        {
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                var claims = user.GetClaims();
                var identity = new ClaimsIdentity(claims, scheme);
                var principal = new ClaimsPrincipal(identity);
                await helper.SignInAsync(scheme, principal);
                return await Task.FromResult(true);
            }
        }
        return await Task.FromResult(false);
    }

    public async Task SendPasswordResetEmail(string email)
    {
        var protector = dataProtectionProvider.CreateProtector("PasswordReset");
        var user = await usersRepository.GetByEmailAsync(email);
        if (user != null)
        {
            protector.Protect(user.Email);
        }
    }

    public async Task LogoutAsync(string scheme)
    {
        await helper.SignOutAsync(scheme);
    }
}
