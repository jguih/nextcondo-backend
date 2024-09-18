using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public interface ICondominiumsRepository : IGenericRepository<Condominium>
{
    public Task<List<CondominiumDTO>> GetDtoListByUserIdAsync(Guid userId);
    public Task<CondominiumDTO?> GetDtoByIdAsync(Guid id);
    public Task<Guid?> GetFirstCondominiumIdForUser(Guid userId);
}

public class CondominiumsRepository : GenericRepository<Condominium>, ICondominiumsRepository
{
    public CondominiumsRepository(
        NextCondoApiDbContext context,
        ILogger<CondominiumsRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<List<CondominiumDTO>> GetDtoListByUserIdAsync(Guid userId)
    {
        var query = from condominium in entities
                    let owner = condominium.Owner
                    let members = condominium.Members
                    where owner.Id == userId ||
                        members.Any(m => m.UserId.Equals(userId))
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
        var list = await query
            .AsNoTracking()
            .ToListAsync();
        return list;
    }

    public async Task<CondominiumDTO?> GetDtoByIdAsync(Guid id)
    {
        var query = from condominium in entities
                    let owner = condominium.Owner
                    let members = condominium.Members
                    where condominium.Id == id
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

    public async Task<Guid?> GetFirstCondominiumIdForUser(Guid userId)
    {
        var condominiums = from condominium in db.Condominiums
                           where condominium.OwnerId.Equals(userId)
                           select new { condominium.Id };
        var condo = await condominiums
            .AsNoTracking()
            .FirstOrDefaultAsync();
        return condo?.Id;
    }
}