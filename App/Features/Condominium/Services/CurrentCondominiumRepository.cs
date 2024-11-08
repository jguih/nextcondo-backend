using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public interface ICurrentCondominiumRepository : IGenericRepository<CurrentCondominium>
{
    /// <summary>
    /// Returns first instance of Condominium as Dto.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<CondominiumDTO?> GetDtoAsync(Guid? userId = default);
    public Task<Guid?> GetCondominiumIdAsync(Guid? userId = default);
    public Task<int> DeleteAsync(Guid? userId = default);
}

public class CurrentCondominiumRepository : GenericRepository<CurrentCondominium>, ICurrentCondominiumRepository
{

    public CurrentCondominiumRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CurrentCondominium>> logger)
        : base(context, logger)
    {
    }

    public async Task<CondominiumDTO?> GetDtoAsync(Guid? userId)
    {
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var query = from currentCondo in entities
                    where !hasUserId
                        || currentCondo.UserId == userId
                    let condominium = currentCondo.Condominium
                    let owner = condominium.Owner
                    let members = condominium.Members
                    select new CondominiumDTO()
                    {
                        Id = condominium.Id,
                        Name = condominium.Name,
                        Description = condominium.Description,
                        Owner = new()
                        {
                            Id = owner.Id,
                            FullName = owner.FullName,
                        },
                        Members = from member in members
                                  select new CondominiumDTO.CondominiumMemberDTO()
                                  {
                                      Id = member.UserId,
                                      FullName = member.User!.FullName,
                                      RelationshipType = member.RelationshipType ==
                                        CondominiumUserRelationshipType.Tenant ?
                                        "Tenant" :
                                        "Manager",
                                  }
                    };
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<Guid?> GetCondominiumIdAsync(Guid? userId)
    {
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var query = from currentCondo in entities
                    where !hasUserId
                        || currentCondo.UserId == userId
                    select currentCondo.CondominiumId;

        return await query
            .FirstOrDefaultAsync();
    }

    public async Task<int> DeleteAsync(Guid? userId = null)
    {
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var query = from currentCondo in entities
                    where currentCondo.UserId.Equals(userId)
                    select currentCondo;
        return await query
            .ExecuteDeleteAsync();
    }
}
