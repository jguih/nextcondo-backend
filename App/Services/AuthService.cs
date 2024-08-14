using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.Validation;
using System.Security.Claims;

namespace NextCondoApi.Services;

public interface IAuthService
{
    public Task<bool> LoginAsync(string email, string password, string schema);
    public Task<bool> RegisterAsync(string fullName, string email, string password, string? phone, string schema);
    public Task SendPasswordResetEmail(string email);
    public Task SignOutAsync(string scheme);
}

public class AuthService : IAuthService
{
    private readonly IUsersRepository usersRepository;
    private readonly IRolesRepository rolesRepository;
    private readonly HttpContext httpContext;
    private readonly IPasswordHasher<User> hasher;
    private readonly IDataProtectionProvider dataProtectionProvider;

    public AuthService(
        IUsersRepository usersRepository,
        IRolesRepository rolesRepository,
        IHttpContextAccessor httpContextAccessor,
        IPasswordHasher<User> hasher,
        IDataProtectionProvider dataProtectionProvider)
    {

        if (httpContextAccessor.HttpContext == null)
        {
            throw new HttpResponseException(
                        new ProblemDetails()
                        {
                            Title = "Authentication service failed",
                            Status = StatusCodes.Status500InternalServerError,
                            Detail = "Failed to create authentication service",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
                        }
                    );
        }

        this.usersRepository = usersRepository;
        this.rolesRepository = rolesRepository;
        this.httpContext = httpContextAccessor.HttpContext;
        this.hasher = hasher;
        this.dataProtectionProvider = dataProtectionProvider;
    }

    public async Task<bool> RegisterAsync(
        string fullName,
        string email,
        string password,
        string? phone,
        string schema)
    {
        var user = new User()
        {
            Email = email,
            FullName = fullName,
            Phone = phone,
            RoleId = (await rolesRepository.GetDefault()).Name,
        };
        var passwordHash = hasher.HashPassword(user, password);
        user.PasswordHash = passwordHash;

        await usersRepository.AddAsync(user);
        await usersRepository.SaveAsync();

        var identity = new ClaimsIdentity(user.GetClaims(), schema);
        var userPrincipal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(schema, userPrincipal);
        return true;
    }

    public async Task<bool> LoginAsync(string email, string password, string schema)
    {
        var user = await usersRepository.GetByEmailAsync(email);
        if (user != null)
        {
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                var claims = user.GetClaims();
                var identity = new ClaimsIdentity(claims, schema);
                var principal = new ClaimsPrincipal(identity);
                await httpContext.SignInAsync(schema, principal);
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

    public async Task SignOutAsync(string scheme)
    {
        await httpContext.SignOutAsync(scheme);
    }
}
