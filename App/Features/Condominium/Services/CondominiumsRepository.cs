﻿using Microsoft.EntityFrameworkCore;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public interface ICondominiumsRepository : IGenericRepository<Condominium>
{
    public Task<List<CondominiumDTO>> GetDtoListAsync(Guid? userId = default);
    public Task<Guid?> GetIdAsync(Guid? userId = default, Guid? id = default);
    public Task<bool> ExistsAsync(Guid? id = null);
}

public class CondominiumsRepository : GenericRepository<Condominium>, ICondominiumsRepository
{
    public CondominiumsRepository(
        NextCondoApiDbContext context,
        ILogger<CondominiumsRepository> logger)
        : base(context, logger)
    {
    }

    public async Task<bool> ExistsAsync(Guid? id = null)
    {
        var hasId = id.HasValue && !id.Value.Equals(Guid.Empty);
        var query = from condominium in entities
                    where !hasId || condominium.Id.Equals(id)
                    select 1;
        return await query.AnyAsync();
    }

    public async Task<List<CondominiumDTO>> GetDtoListAsync(Guid? userId)
    {
        var hasUserId = userId.HasValue && userId.Value != Guid.Empty;

        var query = from condominium in entities
                    let owner = condominium.Owner
                    let members = condominium.Members
                    where !hasUserId
                        || owner.Id == userId
                        || members.Any(m => m.UserId.Equals(userId))
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

    public async Task<Guid?> GetIdAsync(Guid? userId = default, Guid? id = default)
    {
        var hasUserId = userId.HasValue && !userId.Value.Equals(Guid.Empty);
        var hasId = id.HasValue && !id.Value.Equals(Guid.Empty);

        var query = from condominium in db.Condominiums
                    let members = condominium.Members
                    where (!hasUserId
                         || condominium.OwnerId.Equals(userId)
                         || members.Any(m => m.UserId.Equals(userId)))
                         && (!hasId || condominium.Id == id)
                    select condominium.Id;

        return await query.FirstOrDefaultAsync();
    }
}