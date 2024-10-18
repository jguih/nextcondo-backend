using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NextCondoApi.Entity;
using NextCondoApi.Features.Configuration;
using Npgsql;
using Respawn;
using TestFakes;

namespace IntegrationTests.Utils;

public static class DbUtils
{
    private static Faker Faker { get; } = new();

    public static async Task<User> AddTestUserAsync(
        NextCondoApiDbContext db,
        RegisterUserDetails userDetails,
        IPasswordHasher<User> hasher)
    {
        var user = new User()
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Phone = userDetails.Phone,
            RoleId = "User",
        };
        var passwordHash = hasher.HashPassword(user, userDetails.Password);
        user.PasswordHash = passwordHash;

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return user;
    }

    public static async Task<Condominium> AddCondominiumAsync(
        NextCondoApiDbContext db,
        CondominiumDetails details)
    {
        // Create condominium and add user as owner
        Condominium newCondominium = new()
        {
            Id = Faker.Random.Guid(),
            Name = details.Name,
            Description = details.Description,
            OwnerId = details.OwnerId,
        };
        // Add user as a member
        CondominiumUser newMember = new()
        {
            UserId = details.OwnerId,
            RelationshipType = details.RelationshipType,
            CondominiumId = newCondominium.Id,
        };
        newCondominium.Members.Add(newMember);
        await db.Condominiums.AddAsync(newCondominium);
        // Add current condominium for user
        CurrentCondominium current = new()
        {
            CondominiumId = newCondominium.Id,
            UserId = details.OwnerId,
        };
        await db.CurrentCondominium.AddAsync(current);
        await db.SaveChangesAsync();
        return newCondominium;
    }

    public static async Task<CommonArea> AddCommonAreaAsync(
        NextCondoApiDbContext db,
        CommonAreaDetails data)
    {
        CommonArea newCommonArea = new()
        {
            Name = data.Name,
            Description = data.Description,
            CondominiumId = data.CondominiumId,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            TimeInterval = data.TimeInterval
        };
        await db.CommonAreas.AddAsync(newCommonArea);
        await db.SaveChangesAsync();
        return newCommonArea;
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
