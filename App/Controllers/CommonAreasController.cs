using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Features.CommonAreasFeature.Models;
using NextCondoApi.Features.CommonAreasFeature.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace NextCondoApi.Controllers;

[Authorize]
[ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        MediaTypeNames.Application.ProblemJson)]
[Route("[controller]")]
[ApiController]
public class CommonAreasController : ControllerBase
{
    private readonly ICommonAreasService _commonAreasService;

    public CommonAreasController(
        ICommonAreasService commonAreasService)
    {
        _commonAreasService = commonAreasService;
    }


    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status201Created,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [SwaggerOperation(
        summary: "Creates a new common area",
        description: "Creates a new common area for user's current condominium")]
    public async Task<IActionResult> AddAsync([FromForm] CreateCommonAreaCommand data)
    {
        var (result, commonAreaId) = await _commonAreasService.AddAsync(data);
        if (result == CreateCommonAreaResult.NoSlotsProvided)
        {
            return Problem(
                title: "Empty slots array",
                detail: "Slots array can not be empty",
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400"
            );
        }
        if (result == CreateCommonAreaResult.CommonAreaTypeNotFound)
        {
            return Problem(
                title: "Common area type not found",
                detail: "Common area type not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        if (result == CreateCommonAreaResult.Created)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { Id = commonAreaId },
                new { Id = commonAreaId });
        }
        return BadRequest();
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(List<CommonAreaDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        summary: "Returns all common areas",
        description: "Returns all common areas for user's current condominium")]
    public async Task<IActionResult> GetAsync()
    {
        List<CommonAreaDTO> list = await _commonAreasService.GetDtoListAsync();
        return Ok(list);
    }

    [HttpPost("{Id}/reservation")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [SwaggerOperation(
        summary: "Creates a reservation",
        description: "Creates a new reservation on current condominium using specified parameters")]
    public async Task<IActionResult> CreateReservationAsync(int Id, [FromForm] CreateReservationCommand data)
    {
        var (result, _) = await _commonAreasService.CreateReservationAsync(Id, data);
        if (result == CreateReservationResult.CommonAreaNotFound)
        {
            return Problem(
                title: "Common area not found",
                detail: "Common area not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        if (result == CreateReservationResult.SlotNotFound)
        {
            return Problem(
                title: "Slot not found",
                detail: "Slot not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        if (result == CreateReservationResult.InvalidTimeSlot)
        {
            return Problem(
                title: "Invalid time slot",
                detail: "Invalid date or time",
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400"
            );
        }
        if (result == CreateReservationResult.UnavailableTimeSlot)
        {
            return Problem(
                title: "Unavailable time slot",
                detail: "Time slot is already reserved",
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400"
            );
        }
        if (result == CreateReservationResult.Created)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpGet("{Id}")]
    [ProducesResponseType(
        typeof(CommonAreaDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [SwaggerOperation(
        summary: "Returns a common area",
        description: "Returns a common area for user's current condominium with specified Id")]
    public async Task<IActionResult> GetById(int Id)
    {
        var commonArea = await _commonAreasService.GetDtoAsync(Id);
        if (commonArea is null)
        {
            return Problem(
                title: "Common area not found",
                detail: "Common area not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        return Ok(commonArea);
    }

    [HttpGet("{Id}/slot/{SlotId}/bookingSlots")]
    [ProducesResponseType(
        typeof(List<BookingSlot>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [SwaggerOperation(
        summary: "Returns all time slots for common area",
        description: "Returns time slots for the next 7 days for a current condominium's common area with specified Id")]
    public async Task<IActionResult> GetBookingSlotsAsync(int Id, int SlotId)
    {
        var (result, timeSlots) = await _commonAreasService.GetBookingSlotsAsync(Id, SlotId);
        if (result == GetBookingSlotsResult.CommonAreaNotFound)
        {
            return Problem(
                title: "Common area not found",
                detail: "Common area not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        if (result == GetBookingSlotsResult.SlotNotFound)
        {
            return Problem(
               title: "Slot not found",
               detail: "Slot not found",
               statusCode: StatusCodes.Status404NotFound,
               type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
           );
        }
        if (result == GetBookingSlotsResult.Ok)
        {
            return Ok(timeSlots);
        }
        return BadRequest();
    }

    [HttpGet("{Id}/slot/{SlotId}/date/{Date}/bookingSlots")]
    [ProducesResponseType(
        typeof(List<BookingSlot>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [SwaggerOperation(
        summary: "Returns all time slots for common area",
        description: "Returns time slots for the next 7 days for a current condominium's common area with specified Id")]
    public async Task<IActionResult> GetBookingSlotAsync(int Id, int SlotId, DateOnly Date)
    {
        var (result, timeSlot) = await _commonAreasService.GetBookingSlotAsync(Id, SlotId, Date);
        if (result == GetBookingSlotsResult.CommonAreaNotFound)
        {
            return Problem(
                title: "Common area not found",
                detail: "Common area not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        if (result == GetBookingSlotsResult.SlotNotFound)
        {
            return Problem(
               title: "Slot not found",
               detail: "Slot not found",
               statusCode: StatusCodes.Status404NotFound,
               type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
           );
        }
        if (result == GetBookingSlotsResult.Ok)
        {
            return Ok(timeSlot);
        }
        return BadRequest();
    }
}
