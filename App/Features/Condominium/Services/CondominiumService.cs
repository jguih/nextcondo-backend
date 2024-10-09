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
    public Task AddAsync(CreateCondominiumCommand data);
    public Task<List<CondominiumDTO>> GetMineAsync();
    public Task<bool> JoinAsync(JoinCommand data);
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

    public async Task AddAsync(CreateCondominiumCommand data)
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

    public async Task<bool> JoinAsync(JoinCommand data)
    {
        var condominium = await _condominiumsRepository.GetIdAsync(id: data.CondominiumId);

        if (condominium is null || !condominium.HasValue || condominium.Value.Equals(Guid.Empty))
        {
            return false;
        }

        var identity = _currentUserContext.GetIdentity();
        CondominiumUser member = new()
        {
            RelationshipType = data.RelationshipType,
            CondominiumId = condominium.Value,
            UserId = identity,
        };
        await _condominiumUserRepository.AddAsync(member);
        return true;
    }
}
