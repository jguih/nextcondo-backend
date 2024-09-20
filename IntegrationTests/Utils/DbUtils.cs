using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Features.Configuration;
using NextCondoApi.Services;
using Npgsql;
using Respawn;
using TestFakes;

namespace IntegrationTests.Utils;

public static class DbUtils
{
    public static async Task<User> AddTestUserAsync(
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

        return user;
    }

    public static async Task<Condominium> AddCondominiumAsync(
        NextCondoApiDbContext db,
        Guid ownerId,
        CondominiumUserRelationshipType relationshipType)
    {
        // Create condominium and add user as owner
        var condominium = FakeCondominiumsFactory.GetFakeCondominium();
        condominium.OwnerId = ownerId;
        // Add user as a member
        CondominiumUser newMember = new()
        {
            UserId = ownerId,
            RelationshipType = relationshipType,
            CondominiumId = condominium.Id,
        };
        condominium.Members.Add(newMember);
        await db.Condominiums.AddAsync(condominium);
        // Add current condominium for user
        CurrentCondominium current = new()
        {
            CondominiumId = condominium.Id,
            UserId = ownerId,
        };
        await db.CurrentCondominium.AddAsync(current);
        await db.SaveChangesAsync();
        return condominium;
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
                        TablesToIgnore = ["Roles", "__EFMigrationsHistory", "OccurrenceTypes"],
                        DbAdapter = DbAdapter.Postgres
                    }
                );
            await respawner.ResetAsync(conn);
        }
    }
}
