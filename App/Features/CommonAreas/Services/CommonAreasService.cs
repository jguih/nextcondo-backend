
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreasService
{
    public Task<(CreateCommonAreaResult result, int? commonAreaId)> AddAsync(CreateCommonAreaCommand data);
    public Task<List<BookingSlot>?> GetBookingSlotsAsync(int Id);
    public Task<CommonAreaDTO?> GetDtoAsync(int? Id);
    public Task<List<CommonAreaDTO>> GetDtoListAsync();
    public Task<(CreateReservationResult result, int? reservationId)> CreateReservationAsync(int commonAreaId, CreateReservationCommand data);
}

public class CommonAreasService : ICommonAreasService
{
    private readonly ICommonAreasRepository _commonAreasRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;
    private readonly ICommonAreaTypesRepository _commonAreaTypesRepository;
    private readonly IBookingSlotService _timeSlotService;

    public CommonAreasService(
        ICommonAreasRepository commonAreasRepository,
        ICurrentUserContext currentUserContext,
        ICommonAreaReservationsRepository commonAreaReservationsRepository,
        ICommonAreaTypesRepository commonAreaTypesRepository,
        IBookingSlotService timeSlotService)
    {
        _commonAreasRepository = commonAreasRepository;
        _currentUserContext = currentUserContext;
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
        _commonAreaTypesRepository = commonAreaTypesRepository;
        _timeSlotService = timeSlotService;
    }

    public async Task<(CreateCommonAreaResult result, int? commonAreaId)> AddAsync(CreateCommonAreaCommand data)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var typeExists = await _commonAreaTypesRepository.Exists(data.TypeId);
        if (!typeExists)
        {
            return (CreateCommonAreaResult.CommonAreaTypeNotFound, null);
        }
        CommonArea newCommonArea = new()
        {
            TypeId = data.TypeId,
            CondominiumId = currentCondoId,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            TimeInterval = data.TimeInterval
        };
        await _commonAreasRepository.AddAsync(newCommonArea);
        return (CreateCommonAreaResult.Created, newCommonArea.Id);
    }

    public async Task<(CreateReservationResult result, int? reservationId)> CreateReservationAsync(int commonAreaId, CreateReservationCommand data)
    {
        var identity = _currentUserContext.GetIdentity();
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetAsync(
                id: commonAreaId,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return (CreateReservationResult.CommonAreaNotFound, null);
        }

        var timeSlot = await _timeSlotService.GetBookingSlotAsync(commonArea, data.Date);
        var existingSlot = timeSlot.Slots.Find(slot => slot.StartAt.CompareTo(data.StartAt) == 0);

        if (existingSlot is null)
        {
            return (CreateReservationResult.InvalidTimeSlot, null);
        }

        if (!existingSlot.Available)
        {
            return (CreateReservationResult.UnavailableTimeSlot, null);
        }

        CommonAreaReservation newReservation = new()
        {
            UserId = identity,
            CommonAreaId = commonArea.Id,
            Date = data.Date,
            StartAt = data.StartAt
        };

        await _commonAreaReservationsRepository.AddAsync(newReservation);
        return (CreateReservationResult.Created, newReservation.Id);
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

    public async Task<List<BookingSlot>?> GetBookingSlotsAsync(int Id)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetAsync(
                id: Id,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return null;
        }

        int duration = 7;
        List<BookingSlot> bookingSlotList = [];
        DateTime utcNow = DateTime.UtcNow;

        for (int i = 0; i < duration; i++)
        {
            var now = utcNow.AddDays(i);
            var date = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
            var timeSlot = await _timeSlotService.GetBookingSlotAsync(commonArea, date);
            bookingSlotList.Add(timeSlot);
        }

        return bookingSlotList;
    }
}

public enum CreateReservationResult
{
    CommonAreaNotFound,
    InvalidTimeSlot,
    UnavailableTimeSlot,
    Created,
}

public enum CreateCommonAreaResult
{
    CommonAreaTypeNotFound,
    Created,
}