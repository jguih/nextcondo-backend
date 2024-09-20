
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.OccurrencesFeature.Services;
using NextCondoApi.Utils.ClaimsPrincipalExtension;

namespace NextCondoApi.Controllers;

[Authorize]
[ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        MediaTypeNames.Application.ProblemJson)]
[Route("[controller]")]
[ApiController]
public class OccurrencesController : ControllerBase
{
    private readonly IOccurrenceTypesRepository _occurrenceTypesRepository;
    private readonly OccurrencesService _occurrencesService;

    public OccurrencesController(
        IOccurrenceTypesRepository occurrenceTypesRepository,
        OccurrencesService occurrencesService)
    {
        _occurrenceTypesRepository = occurrenceTypesRepository;
        _occurrencesService = occurrencesService;
    }

    [HttpGet("types")]
    [ProducesResponseType(
        typeof(List<OccurrenceTypeDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetOccurrenceTypesAsync()
    {
        var typesList = await _occurrenceTypesRepository.GetDtoListAsync();
        return Ok(typesList);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status201Created,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> AddAsync([FromForm] AddOccurrenceDTO data)
    {
        var occurrence = await _occurrencesService.AddAsync(data, User.GetIdentity());

        if (occurrence is null)
        {
            return Problem(
                title: "Current condominium not found",
                detail: "User does not have a current condominium",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }

        return CreatedAtAction(nameof(GetById), new { occurrence.Id }, new { occurrence.Id });
    }

    [HttpGet("{Id}")]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(
        typeof(OccurrenceDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetById(Guid Id)
    {
        var result = await _occurrencesService.GetByIdAsync(Id, User.GetIdentity());

        if (result.result == 1)
        {
            return Problem(
                title: "Occurrence not found",
                detail: $"Occurrence {Id} not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }

        if (result.result == 2)
        {
            return Problem(
                title: "Current condominium not found",
                detail: "User does not have a current condominium",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }

        return Ok(result.occurrence);
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(List<OccurrenceDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetCurrentAsync()
    {
        var identity = User.GetIdentity();
        var result = await _occurrencesService.GetCurrentAsync(identity);
        return Ok(result);
    }
}