
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Features.TenantsFeature.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace NextCondoApi.Controllers;

[Authorize]
[ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        MediaTypeNames.Application.ProblemJson)]
[Route("[controller]")]
[ApiController]
public class TenantsController : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(summary: "Returns all tenants for current condominium", description: "Returns a list of all tenants for current user's current condominium")]
    [ProducesResponseType(
        typeof(List<TenantDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAsync()
    {
        return Ok();
    }
}