using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Controllers.UsersControllerDTO;
using NextCondoApi.Services;
using NextCondoApi.Utils.ClaimsPrincipalExtension;

namespace NextCondoApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersRepository users;

    public UsersController(IUsersRepository users)
    {
        this.users = users;
    }

    [HttpPut]
    public async Task<IActionResult> EditAsync([FromBody] EditUserDTO newUser)
    {
        var id = User.GetIdentity();
        var existing = await users.GetByIdAsync(id);

        if (existing == null)
        {
            return BadRequest();
        }

        if (newUser.FullName != null)
        {
            existing.FullName = newUser.FullName;
        }

        if (newUser.Phone != null)
        {
            existing.Phone = newUser.Phone;
        }

        await users.SaveAsync();

        return Ok();
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var users = (await this.users.GetAllAsync())
            .Select(user => new UserDTO()
            {
                Id = user.Id,
                Email = user.Email,
                Role = new UserDTO.UserRole()
                {
                    Name = user.Role!.Name,
                }
            });
        if (users.Any())
        {
            return Ok(users);
        }
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMeAsync()
    {
        var id = HttpContext.User.GetIdentity();
        var user = await users.GetByIdAsync(id);
        return user != null ? Ok(user) : NoContent();
    }
}
