using NextCondoApi.Services.Auth;
using System.Security.Claims;

namespace UnitTests.Fakes;

public class FakeAuthServiceHelper : IAuthServiceHelper
{
    public Task SignInAsync(string scheme, ClaimsPrincipal principal)
    {
        return Task.CompletedTask;
    }

    public Task SignOutAsync(string scheme)
    {
        return Task.CompletedTask;
    }
}