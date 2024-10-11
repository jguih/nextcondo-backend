using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Features.CondominiumFeature.Models;
using System.Net.Mime;
using Swashbuckle.AspNetCore.Annotations;

namespace NextCondoApi.Controllers;

[Authorize]
[ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        MediaTypeNames.Application.ProblemJson)]
[Route("[controller]")]
[ApiController]
public class CondominiumController : ControllerBase
{
    private readonly ICondominiumService _condominiumService;

    public CondominiumController(ICondominiumService condominiumService)
    {
        _condominiumService = condominiumService;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAsync([FromForm] CreateCondominiumCommand data)
    {
        var Id = await _condominiumService.AddAsync(data);
        return CreatedAtAction(nameof(GetMine), new { Id }, new { Id });
    }

    [HttpGet("mine")]
    [ProducesResponseType(
        typeof(List<CondominiumDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetMine()
    {
        var userCondominiums = await _condominiumService.GetMineAsync();
        return Ok(userCondominiums);
    }

    [HttpGet("mine/current")]
    [ProducesResponseType(
        typeof(CondominiumDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetMineCurrentAsync()
    {
        var current = await _condominiumService.GetCurrentAsync();

        if (current is not null)
        {
            return Ok(current);
        }

        return NoContent();
    }

    [HttpPost("join")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest,
        MediaTypeNames.Application.ProblemJson)]
    [SwaggerOperation(
        summary: "Joins a condominium",
        description: "Joins a condominium as current user using specified relationship type")]
    public async Task<IActionResult> JoinAsync([FromForm] JoinCommand data)
    {
        var result = await _condominiumService.JoinAsync(data);
        if (result == 2)
        {
            return Problem(
                title: "Condominium not found",
                detail: "Condominium not found",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
            );
        }
        if (result == 1)
        {
            return Problem(
                title: "User is already a member",
                detail: $"Failed to create relationship. User is already a member of condominium with Id {data.CondominiumId}",
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400"
            );
        }
        return Ok();
    }
}
