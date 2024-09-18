using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using TestFakes;

namespace IntegrationTests.Utils;

public static class TestsWebApplicationFactoryExtension
{
    public static async Task<(User user, HttpClient client)> CreateAuthenticatedHttpClientForUserAsync(
        this TestsWebApplicationFactory<Program> factory, 
        RegisterUserDetails userDetails)
    {
        User testUser;
        using (var scope = factory.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var users = provider.GetRequiredService<IUsersRepository>();
            var roles = provider.GetRequiredService<IRolesRepository>();
            var hasher = provider.GetRequiredService<IPasswordHasher<User>>();
            testUser = await DbUtils.AddTestUserAsync(userDetails, users, roles, hasher);
        };
        var client = factory.CreateClient();
        await client.LoginAsync(userDetails.Email, userDetails.Password);
        return (testUser, client);
    }
}
