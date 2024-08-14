using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.Validation;
using static System.Net.WebRequestMethods;

namespace NextCondoApi.Services;

public interface IRolesRepository : IGenericRepository<Role>
{
    public Task<Role> GetDefault();
};

public class RolesRepository : GenericRepository<Role>, IRolesRepository
{
    public RolesRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<Role>> logger)
        : base(context, logger)
    {
    }

    public async Task<Role> GetDefault()
    {
        var defaultRole = await GetByIdAsync("Tenant");
        
        if (defaultRole == null)
        {
            throw new HttpResponseException(new ProblemDetails()
            {
                Title = "Tenant role doesn't exist",
                Detail = "Tenant role could not be found in the database",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
            });
        }

        return defaultRole;
    }
}
