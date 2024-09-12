using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using NextCondoApi.Services;
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
    private readonly IUsersRepository _usersRepository;

    public CondominiumController(
        ICondominiumsRepository condominiumRepository,
        IUsersRepository usersRepository)
    {
        _condominiumsRepository = condominiumRepository;
        _usersRepository = usersRepository;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> AddAsync([FromForm] AddCondominiumDTO data)
    {
        var owner = await _usersRepository.GetByIdAsync(data.OwnerId);

        if (owner == null)
        {
            return Problem(
                title: "Owner not found",
                detail: "Could not find owner using provided Id",
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        Condominium newCondominium = new()
        {
            Name = data.Name,
            Description = data.Description,
            OwnerId = data.OwnerId,
            Owner = owner,
        };

        CondominiumUser newMember = new()
        {
            Condominium = newCondominium,
            CondominiumId = newCondominium.Id,
            RelationshipType = data.RelationshipType,
            User = owner,
            UserId = owner.Id,
        };

        newCondominium.Members.Add(newMember);

        await _condominiumsRepository.AddAsync(newCondominium);
        await _condominiumsRepository.SaveAsync();

        return CreatedAtAction(
            nameof(GetById), 
            new { Id = newCondominium.Id }, 
            CondominiumDTO.FromCondominium(newCondominium));
    }

    [HttpGet("mine")]
    [ProducesResponseType(
        typeof(List<CondominiumDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetMineAsync()
    {
        var identity = User.GetIdentity();
        List<Condominium> myCondos = await _condominiumsRepository.GetByOwnerIdAsync(identity);
        List<CondominiumDTO> myCondosDtos =
            (from condominium in myCondos
             select CondominiumDTO.FromCondominium(condominium))
             .ToList();

        return Ok(myCondosDtos);
    }

    [HttpGet("{Id}")]
    [ProducesResponseType(
        typeof(CondominiumDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> GetById(Guid Id)
    {
        Condominium? condo = await _condominiumsRepository.GetByIdAsync(Id);
        return condo is not null ? Ok(CondominiumDTO.FromCondominium(condo)) : 
            Problem(
                title: "Condominium not found",
                detail: "Could not find condominium using provided Id",
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404",
                statusCode: StatusCodes.Status404NotFound
            );
    }
}
