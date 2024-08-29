using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using Npgsql;
using Respawn;

namespace IntegrationTests.Utils;

public static class DbUtils
{
    public static async Task AddTestUserAsync(IUsersRepository users, IRolesRepository roles, IPasswordHasher<User> hasher)
    {
        Role defaultRole = await roles.GetDefaultAsync();
        var user = new User()
        {
            Email = "test@test.com",
            FullName = "Test User",
            Phone = "123456",
            RoleId = defaultRole.Name,
            Role = defaultRole,
        };
        var passwordHash = hasher.HashPassword(user, "test12345");
        user.PasswordHash = passwordHash;

        await users.AddAsync(user);
        await users.SaveAsync();
    }

    public static async Task CleanUpAsync(IConfiguration configuration)
    {
        var connectionString = NextCondoApiDbContext.GetConnectionString(configuration);

        using (var conn = new NpgsqlConnection(connectionString))
        {
            await conn.OpenAsync();
            var respawner = await Respawner
                .CreateAsync(
                    conn,
                    new RespawnerOptions
                    {
                        SchemasToInclude = ["public"],
                        TablesToIgnore = ["Roles", "__EFMigrationsHistory"],
                        DbAdapter = DbAdapter.Postgres
                    }
                );
            await respawner.ResetAsync(conn);
        }
    }
}
