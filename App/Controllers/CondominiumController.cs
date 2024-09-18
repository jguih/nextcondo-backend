using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Models.DTO;
using NextCondoApi.Utils.ClaimsPrincipalExtension;
using System.Net.Mime;

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
    private readonly ICondominiumsRepository _condominiumsRepository;
    private readonly ICurrentCondominiumRepository _currentCondominiumRepository;
    private readonly CondominiumService _condominiumService;

    public CondominiumController(
        ICondominiumsRepository condominiumRepository,
        ICurrentCondominiumRepository currentCondominiumRepository,
        CondominiumService condominiumService)
    {
        _condominiumsRepository = condominiumRepository;
        _currentCondominiumRepository = currentCondominiumRepository;
        _condominiumService = condominiumService;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> AddAsync([FromForm] AddCondominiumDTO data)
    {
        var identity = User.GetIdentity();

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

        return Ok();
    }

    [HttpGet("mine")]
    [ProducesResponseType(
        typeof(List<CondominiumDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetMineAsync()
    {
        var identity = User.GetIdentity();
        var userCondominiums = await _condominiumsRepository.GetDtoListByUserIdAsync(identity);
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
        var identity = User.GetIdentity();
        var current = await _condominiumService.GetCurrentAsync(identity);

        if (current is not null)
        {
            return Ok(current);
        }

        return NoContent();
    }
}
