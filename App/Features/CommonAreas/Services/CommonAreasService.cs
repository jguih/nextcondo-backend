
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreasService
{
    public Task<int> AddAsync(CreateCommonAreaCommand data);
    public Task<List<TimeSlot>?> GetTimeSlotsAsync(int Id);
    public Task<CommonAreaDTO?> GetDtoAsync(int? Id);
    public Task<List<CommonAreaDTO>> GetDtoListAsync();
}

public class CommonAreasService : ICommonAreasService
{
    private readonly ICommonAreasRepository _commonAreasRepository;
    private readonly ICurrentUserContext _currentUserContext;

    public CommonAreasService(
        ICommonAreasRepository commonAreasRepository,
        ICurrentUserContext currentUserContext)
    {
        _commonAreasRepository = commonAreasRepository;
        _currentUserContext = currentUserContext;
    }

    public async Task<int> AddAsync(CreateCommonAreaCommand data)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        CommonArea newCommonArea = new()
        {
            Name = data.Name,
            Description = data.Description,
            CondominiumId = currentCondoId,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            TimeInterval = data.TimeInterval
        };
        await _commonAreasRepository.AddAsync(newCommonArea);
        return newCommonArea.Id;
    }

    public async Task<CommonAreaDTO?> GetDtoAsync(int? Id)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        return await _commonAreasRepository
            .GetDtoAsync(
                id: Id,
                condominiumId: currentCondoId);
    }

    public async Task<List<CommonAreaDTO>> GetDtoListAsync()
    {
        var dtoList = await _commonAreasRepository.GetDtoListAsync();
        return dtoList;
    }

    public async Task<List<TimeSlot>?> GetTimeSlotsAsync(int Id)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetDtoAsync(
                id: Id,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return null;
        }

        int duration = 7;
        List<TimeSlot> timeSlotList = [];
        DateTime now = DateTime.UtcNow;

        for (int i = 0; i < duration; i++)
        {
            now = now.AddDays(i);
            var date = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);

            TimeSlot timeSlot = new()
            {
                Date = date
            };

            var startAt = commonArea.StartTime;
            var maxTimeSlots = 100;
            var timeSlotsIndex = 0;
            while (startAt.CompareTo(commonArea.EndTime) <= 0 || timeSlotsIndex == maxTimeSlots)
            {
                Slot slot = new()
                {
                    StartAt = startAt,
                    Available = true,
                };
                timeSlot.Slots.Add(slot);
                startAt = startAt.Add(commonArea.TimeInterval.ToTimeSpan());
                timeSlotsIndex++;
            }

            timeSlotList.Add(timeSlot);
        }

        return timeSlotList;
    }
}