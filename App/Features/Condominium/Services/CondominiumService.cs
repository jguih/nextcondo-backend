using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CondominiumFeature.Services;

public interface ICondominiumService
{
    /// <summary>
    /// Returns user's current condominium
    /// </summary>
    /// <returns></returns>
    public Task<CondominiumDTO?> GetCurrentAsync();
    public Task<Guid> AddAsync(CreateCondominiumCommand data);
    public Task<List<CondominiumDTO>> GetMineAsync();
    public Task<int> JoinAsync(JoinCommand data);
    public Task<(SetCurrentResult result, CondominiumDTO? current)> SetCurrentAsync(Guid condominiumId);
}

public class CondominiumService : ICondominiumService
{
    private readonly ICurrentCondominiumRepository _currentCondominiumRepository;
    private readonly ICondominiumsRepository _condominiumsRepository;
    private readonly ICondominiumUserRepository _condominiumUserRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public CondominiumService(
        ICurrentCondominiumRepository currentCondominiumRepository,
        ICondominiumsRepository condominiumsRepository,
        ICurrentUserContext currentUserContext,
        ICondominiumUserRepository condominiumUserRepository)
    {
        _currentCondominiumRepository = currentCondominiumRepository;
        _condominiumsRepository = condominiumsRepository;
        _currentUserContext = currentUserContext;
        _condominiumUserRepository = condominiumUserRepository;
    }

    public async Task<Guid> AddAsync(CreateCondominiumCommand data)
    {
        var identity = _currentUserContext.GetIdentity();
        Condominium newCondominium = new()
        {
            Name = data.Name,
            Description = data.Description,
            OwnerId = identity,
        };
        CondominiumUser newMember = new()
        {
            CondominiumId = newCondominium.Id,
            RelationshipType = data.RelationshipType,
            UserId = identity,
        };
        newCondominium.Members.Add(newMember);
        await _condominiumsRepository.AddAsync(newCondominium);
        return newCondominium.Id;
    }

    public async Task<CondominiumDTO?> GetCurrentAsync()
    {
        var userId = _currentUserContext.GetIdentity();
        var current = await _currentCondominiumRepository.GetDtoAsync(userId);

        if (current is not null)
        {
            return current;
        }

        var firstCondominiumId = await _condominiumsRepository.GetIdAsync(userId);

        if (!firstCondominiumId.HasValue || firstCondominiumId.Value.Equals(Guid.Empty))
        {
            return null;
        }

        CurrentCondominium newCurrent = new()
        {
            CondominiumId = firstCondominiumId.Value,
            UserId = userId,
        };

        await _currentCondominiumRepository.AddAsync(newCurrent);

        return await _currentCondominiumRepository.GetDtoAsync(userId);
    }

    public async Task<List<CondominiumDTO>> GetMineAsync()
    {
        var identity = _currentUserContext.GetIdentity();
        var userCondominiums = await _condominiumsRepository.GetDtoListAsync(identity);
        return userCondominiums;
    }

    public async Task<int> JoinAsync(JoinCommand data)
    {
        var condominiumId = await _condominiumsRepository.GetIdAsync(id: data.CondominiumId);

        if (condominiumId is null || !condominiumId.HasValue || condominiumId.Value.Equals(Guid.Empty))
        {
            return 2;
        }

        var identity = _currentUserContext.GetIdentity();
        var membershipExists = await _condominiumUserRepository.ExistsAsync(identity, condominiumId);

        if (membershipExists)
        {
            return 1;
        }

        CondominiumUser membership = new()
        {
            RelationshipType = data.RelationshipType,
            CondominiumId = condominiumId.Value,
            UserId = identity,
        };
        await _condominiumUserRepository.AddAsync(membership);

        return 0;
    }

    public async Task<(SetCurrentResult result, CondominiumDTO? current)> SetCurrentAsync(Guid condominiumId)
    {
        var identity = _currentUserContext.GetIdentity();
        var exists = await _condominiumsRepository.ExistsAsync(condominiumId);
        if (!exists)
        {
            return (SetCurrentResult.CondominiumNotFound, null);
        }
        await _currentCondominiumRepository.DeleteAsync(userId: identity);
        CurrentCondominium newCurrent = new()
        {
            CondominiumId = condominiumId,
            UserId = identity,
        };
        await _currentCondominiumRepository.AddAsync(newCurrent);
        var dto = await _currentCondominiumRepository.GetDtoAsync(identity);
        return (SetCurrentResult.Ok, dto);
    }
}

public enum SetCurrentResult
{
    CondominiumNotFound,
    Ok
}