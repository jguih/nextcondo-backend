using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace NextCondoApi.Services.Auth;

public interface IAuthServiceHelper
{
    public Task SignInAsync(string scheme, ClaimsPrincipal principal);
    public Task SignOutAsync(string scheme);
}

public class AuthServiceHelper : IAuthServiceHelper
{
    private readonly HttpContext httpContext;

    public AuthServiceHelper(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        this.httpContext = httpContextAccessor.HttpContext;
    }

    public async Task SignInAsync(string scheme, ClaimsPrincipal principal)
    {
        await httpContext.SignInAsync(scheme, principal);
    }

    public async Task SignOutAsync(string scheme)
    {
        await httpContext.SignOutAsync(scheme);
    }
}
