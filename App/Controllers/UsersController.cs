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
    private readonly IUsersRepository repository;

    public UsersController(IUsersRepository repository)
    {
        this.repository = repository;
    }

    [HttpPut]
    public async Task<IActionResult> EditAsync([FromBody] EditUserDTO newUser)
    {
        var id = User.GetIdentity();
        var existing = await repository.GetByIdAsync(id);

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

        await repository.SaveAsync();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = (await repository.GetAllAsync())
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
}
