using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NextCondoApi.Entity;
using System.Collections.Generic;

namespace IntegrationTests.Utils;

public static class DbUtils
{
    public static async Task AddTestUserAsync(NextCondoApiDbContext db, IPasswordHasher<User> hasher)
    {
        var user = new User()
        {
            Email = "test@test.com",
            FullName = "Test User",
            Phone = "123456",
            RoleId = "Tenant",
        };
        var passwordHash = hasher.HashPassword(user, "test12345");
        user.PasswordHash = passwordHash;

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
    }

    public static async Task RemoveUsersAsync(NextCondoApiDbContext db)
    {
        db.Users.RemoveRange(db.Users);
        await db.SaveChangesAsync();
    }
}
