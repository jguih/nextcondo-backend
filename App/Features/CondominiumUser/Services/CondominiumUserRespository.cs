
using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.TenantsFeature.Models;
using NextCondoApi.Services;

public interface ICondominiumUserRepository : IGenericRepository<CondominiumUser>
{
    public Task<List<TenantDTO>> GetTenantListAsync(Guid? condominiumId = default);
    public Task<bool> ExistsAsync(Guid? userId = default, Guid? condominiumId = default);
}

public class CondominiumUserRepository : GenericRepository<CondominiumUser>, ICondominiumUserRepository
{
    public CondominiumUserRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CondominiumUser>> logger)
        : base(context, logger)
    {
    }

    public async Task<bool> ExistsAsync(Guid? userId = null, Guid? condominiumId = null)
    {
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var query = from condoUser in entities
                    where (!hasUserId || userId == condoUser.UserId)
                        && (!hasCondominiumId || condominiumId == condoUser.CondominiumId)
                    select 1;
        return await query.AnyAsync();
    }

    public async Task<List<TenantDTO>> GetTenantListAsync(Guid? condominiumId = null)
    {
        var hasCondominiumId = condominiumId.HasValue && !condominiumId.Value.Equals(Guid.Empty);
        var query = from condoUser in entities
                    let user = condoUser.User
                    let relationshipType =
                        condoUser.RelationshipType == CondominiumUserRelationshipType.Manager ? "Manager" :
                        condoUser.RelationshipType == CondominiumUserRelationshipType.Tenant ? "Tenant" :
                        ""
                    where !hasCondominiumId || condominiumId!.Value == condoUser.CondominiumId
                    select new TenantDTO()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Phone = user.Phone,
                        RelationshipType = relationshipType,
                    };
        return await query
            .AsNoTracking()
            .ToListAsync();
    }
}