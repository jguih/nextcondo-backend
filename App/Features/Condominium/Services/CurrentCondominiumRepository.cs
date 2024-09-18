using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public interface ICurrentCondominiumRepository : IGenericRepository<CurrentCondominium>
{
    /// <summary>
    /// Returns an instance of CondominiumDto of current condominium for User.
    /// </summary>
    /// <param name="userId">Current User's Id</param>
    /// <returns>CondominiumDTO</returns>
    public Task<CondominiumDTO?> GetDtoByUserIdAsync(Guid userId);
}

public class CurrentCondominiumRepository : GenericRepository<CurrentCondominium>, ICurrentCondominiumRepository
{

    public CurrentCondominiumRepository(
        NextCondoApiDbContext context,
        ILogger<GenericRepository<CurrentCondominium>> logger)
        : base(context, logger)
    {
    }

    public async Task<CondominiumDTO?> GetDtoByUserIdAsync(Guid userId)
    {
        var query = from currentCondo in entities
                    where currentCondo.UserId == userId
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
        return await query.AsNoTracking().FirstOrDefaultAsync();
    }
}
