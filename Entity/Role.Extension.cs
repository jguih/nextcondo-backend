using NextCondoApi.Features.Validation;

namespace NextCondoApi.Entity;

public static class RoleExtension
{
    public static async Task<Role> GetDefaultRole(this SimplifyCondoApiDbContext db)
    {
        var defaultRole = await db.Roles.FindAsync("Tenant");

        if (defaultRole == null)
        {
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, "Could not find role 'Tenant' on database");
        }

        return defaultRole;
    }
}
