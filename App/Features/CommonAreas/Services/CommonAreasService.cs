
using NextCondoApi.Entity;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Services;

namespace NextCondoApi.Features.CommonAreasFeature.Services;

public interface ICommonAreasService
{
    public Task<(CreateCommonAreaResult result, int? commonAreaId)> AddAsync(CreateCommonAreaCommand data);
    public Task<(GetBookingSlotsResult result, List<BookingSlot>? slotList)> GetBookingSlotsAsync(int Id, int slotId);
    public Task<(GetBookingSlotsResult result, BookingSlot? slotList)> GetBookingSlotAsync(int Id, int slotId, DateOnly date);
    public Task<CommonAreaDTO?> GetDtoAsync(int? Id);
    public Task<List<CommonAreaDTO>> GetDtoListAsync();
    public Task<(CreateReservationResult result, int? reservationId)> CreateReservationAsync(int commonAreaId, CreateReservationCommand data);
    public Task<List<CommonAreaReservationDTO>> GetReservationsAsync();
}

public class CommonAreasService : ICommonAreasService
{
    private readonly ICommonAreasRepository _commonAreasRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ICommonAreaReservationsRepository _commonAreaReservationsRepository;
    private readonly ICommonAreaTypesRepository _commonAreaTypesRepository;
    private readonly IBookingSlotService _bookingSlotService;

    public CommonAreasService(
        ICommonAreasRepository commonAreasRepository,
        ICurrentUserContext currentUserContext,
        ICommonAreaReservationsRepository commonAreaReservationsRepository,
        ICommonAreaTypesRepository commonAreaTypesRepository,
        IBookingSlotService bookingSlotService)
    {
        _commonAreasRepository = commonAreasRepository;
        _currentUserContext = currentUserContext;
        _commonAreaReservationsRepository = commonAreaReservationsRepository;
        _commonAreaTypesRepository = commonAreaTypesRepository;
        _bookingSlotService = bookingSlotService;
    }

    public async Task<(CreateCommonAreaResult result, int? commonAreaId)> AddAsync(CreateCommonAreaCommand data)
    {
        if (data.Slots.Count == 0)
        {
            return (CreateCommonAreaResult.NoSlotsProvided, null);
        }
        var typeExists = await _commonAreaTypesRepository.Exists(data.TypeId);
        if (!typeExists)
        {
            return (CreateCommonAreaResult.CommonAreaTypeNotFound, null);
        }
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        CommonArea newCommonArea = new()
        {
            TypeId = data.TypeId,
            CondominiumId = currentCondoId,
            Slots = data.GetSlots(),
        };
        await _commonAreasRepository.AddAsync(newCommonArea);
        return (CreateCommonAreaResult.Created, newCommonArea.Id);
    }

    public async Task<(CreateReservationResult result, int? reservationId)> CreateReservationAsync(
        int commonAreaId,
        CreateReservationCommand data)
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

        var existingSlot = commonArea.Slots
            .ToList()
            .Find(slot => slot.Id.Equals(data.SlotId));

        if (existingSlot is null)
        {
            return (CreateReservationResult.SlotNotFound, null);
        }

        var bookingSlot = await _bookingSlotService.GetBookingSlotAsync(commonArea, data.Date, data.SlotId);
        var existingBookingSlot = bookingSlot.Slots.Find(slot => slot.StartAt.CompareTo(data.StartAt) == 0);

        if (existingBookingSlot is null)
        {
            return (CreateReservationResult.InvalidTimeSlot, null);
        }

        if (!existingBookingSlot.Available)
        {
            return (CreateReservationResult.UnavailableTimeSlot, null);
        }

        CommonAreaReservation newReservation = new()
        {
            UserId = identity,
            CommonAreaId = commonArea.Id,
            Date = data.Date,
            StartAt = data.StartAt,
            SlotId = existingSlot.Id,
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

    public async Task<(GetBookingSlotsResult result, List<BookingSlot>? slotList)> GetBookingSlotsAsync(
        int Id, int slotId)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetAsync(
                id: Id,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return (GetBookingSlotsResult.CommonAreaNotFound, null);
        }

        var existingSlot = commonArea.Slots
            .ToList()
            .Find(slot => slot.Id.Equals(slotId));

        if (existingSlot is null)
        {
            return (GetBookingSlotsResult.SlotNotFound, null);
        }

        int duration = 7;
        List<BookingSlot> bookingSlotList = [];
        DateTime utcNow = DateTime.UtcNow;

        for (int i = 0; i < duration; i++)
        {
            var now = utcNow.AddDays(i);
            var date = new DateOnly(now.Date.Year, now.Date.Month, now.Date.Day);
            var timeSlot = await _bookingSlotService.GetBookingSlotAsync(commonArea, date, slotId);
            bookingSlotList.Add(timeSlot);
        }

        return (GetBookingSlotsResult.Ok, bookingSlotList);
    }

    public async Task<(GetBookingSlotsResult result, BookingSlot? slotList)> GetBookingSlotAsync(
        int Id, int slotId, DateOnly date)
    {
        var currentCondoId = await _currentUserContext.GetCurrentCondominiumIdAsync();
        var commonArea = await _commonAreasRepository
            .GetAsync(
                id: Id,
                condominiumId: currentCondoId);

        if (commonArea is null)
        {
            return (GetBookingSlotsResult.CommonAreaNotFound, null);
        }

        var existingSlot = commonArea.Slots
            .ToList()
            .Find(slot => slot.Id.Equals(slotId));

        if (existingSlot is null)
        {
            return (GetBookingSlotsResult.SlotNotFound, null);
        }

        var bookingSlot = await _bookingSlotService.GetBookingSlotAsync(commonArea, date, slotId);
        return (GetBookingSlotsResult.Ok, bookingSlot);
    }

    public async Task<List<CommonAreaReservationDTO>> GetReservationsAsync()
    {
        var identity = _currentUserContext.GetIdentity();
        var dtoList = await _commonAreaReservationsRepository.GetDtoListAsync(identity);
        return dtoList;
    }
}

public enum CreateReservationResult
{
    CommonAreaNotFound,
    SlotNotFound,
    InvalidTimeSlot,
    UnavailableTimeSlot,
    Created,
}

public enum CreateCommonAreaResult
{
    CommonAreaTypeNotFound,
    NoSlotsProvided,
    Created,
}

public enum GetBookingSlotsResult
{
    CommonAreaNotFound,
    SlotNotFound,
    Ok
}