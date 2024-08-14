using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Features.Validation;

namespace NextCondoApi.Entity;

public static class RoleExtension
{
    public static async Task<Role> GetDefaultRole(this NextCondoApiDbContext db)
    {
        var defaultRole = await db.Roles.FindAsync("Tenant");

        if (defaultRole == null)
        {
            throw new HttpResponseException(new ProblemDetails()
            {
                Title = "No default role",
                Detail = "Could not find role 'Tenant' on database",
                Status = StatusCodes.Status500InternalServerError,
                Type = "",
            });
        }

        return defaultRole;
    }
}
