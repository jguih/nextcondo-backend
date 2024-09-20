using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
public class UsersController : ControllerBase
{
    private readonly IUsersRepository _usersRepository;

    public UsersController(IUsersRepository usersRepository)
    {
        this._usersRepository = usersRepository;
    }

    [HttpGet("me")]
    [ProducesResponseType(
        typeof(UserDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound,
        MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> GetMeAsync()
    {
        var identity = User.GetIdentity();
        var user = await _usersRepository.GetDtoByUserIdAsync(User.GetIdentity());

        if (user == null)
        {
            return Problem(
                        title: "User not found",
                        detail: $"User with id {identity} not found",
                        statusCode: StatusCodes.Status404NotFound,
                        type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
                    );
        }

        return Ok(user);
    }
}
