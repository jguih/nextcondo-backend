using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NextCondoApi.Entity;
using NextCondoApi.Features.Configuration;
using NextCondoApi.Services;
using Npgsql;
using Respawn;
using TestFakes;

namespace IntegrationTests.Utils;

public static class DbUtils
{
    public static async Task<Guid> AddTestUserAsync(
        RegisterUserDetails userDetails,
        IUsersRepository users, 
        IRolesRepository roles, 
        IPasswordHasher<User> hasher)
    {
        Role defaultRole = await roles.GetDefaultAsync();
        var user = new User()
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Phone = userDetails.Phone,
            RoleId = defaultRole.Name,
            Role = defaultRole,
        };
        var passwordHash = hasher.HashPassword(user, userDetails.Password);
        user.PasswordHash = passwordHash;

        await users.AddAsync(user);

        return user.Id;
    }

    public static async Task CleanUpAsync(IOptions<DbOptions> configuration)
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
