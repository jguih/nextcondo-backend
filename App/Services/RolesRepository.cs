using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.Validation;

namespace NextCondoApi.Services;

public interface IRolesRepository : IGenericRepository<Role>
{
    public Task<Role> GetDefaultAsync();
};

public class RolesRepository : GenericRepository<Role>, IRolesRepository
{
    public RolesRepository(
        NextCondoApiDbContext context,
        ILogger<RolesRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<Role> GetDefaultAsync()
    {
        var defaultRole = await GetByIdAsync("User");
        
        if (defaultRole == null)
        {
            throw new HttpResponseException(new ProblemDetails()
            {
                Title = "User role doesn't exist",
                Detail = "User role could not be found in the database",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
            });
        }

        return defaultRole;
    }
}
