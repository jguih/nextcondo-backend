using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Models.DTO;
using NextCondoApi.Services;
using NextCondoApi.Utils.ClaimsPrincipalExtension;
using NextCondoApi.Entity;
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
    private readonly IUsersRepository users;

    public UsersController(IUsersRepository users)
    {
        this.users = users;
    }

    //[HttpPut]
    //public async Task<IActionResult> EditAsync([FromForm] EditUserDTO newUser)
    //{
    //    var id = User.GetIdentity();
    //    var existing = await users.GetByIdAsync(id);

    //    if (existing == null)
    //    {
    //        return Problem(
    //                    title: "User doesn't exist",
    //                    detail: $"Could not find user with id {id}",
    //                    type: "",
    //                    statusCode: StatusCodes.Status404NotFound
    //                );
    //    }

    //    if (newUser.FullName != null)
    //    {
    //        existing.FullName = newUser.FullName;
    //    }

    //    if (newUser.Phone != null)
    //    {
    //        existing.Phone = newUser.Phone;
    //    }

    //    await users.SaveAsync();

    //    return Ok();
    //}

    [HttpGet("all")]
    [ProducesResponseType(
        typeof(List<UserDTO>),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAll()
    {
        List<User> users = await this.users.GetAllAsync();
        List<UserDTO> usersDtos = (from user in users
                                   select UserDTO.FromUser(user))
                                   .ToList();
            
        return Ok(usersDtos);
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
        var id = HttpContext.User.GetIdentity();
        var user = await users.GetByIdAsync(id);

        if (user == null)
        {
            return Problem(
                        title: "User not found",
                        detail: $"User with id {id} not found",
                        statusCode: StatusCodes.Status404NotFound,
                        type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404"
                    );
        }

        UserDTO userDto = UserDTO.FromUser(user);
        return Ok(userDto);
    }
}
