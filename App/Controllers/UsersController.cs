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
    public async Task<IResult> EditAsync([FromBody] EditUserDTO newUser)
    {
        var id = User.GetIdentity();
        var existing = await users.GetByIdAsync(id);

        if (existing == null)
        {
            return TypedResults.Problem(
                        title: "User doesn't exist",
                        detail: $"Could not find user with id {id}",
                        type: "",
                        statusCode: StatusCodes.Status400BadRequest
                    );
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

        return TypedResults.Extensions.NoContent();
    }

    [HttpGet("all")]
    public async Task<IResult> GetAll()
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
            return TypedResults.Ok(users);
        }
        return TypedResults.Extensions.NoContent();
    }

    [HttpGet("me")]
    public async Task<IResult> GetMeAsync()
    {
        var id = HttpContext.User.GetIdentity();
        var user = await users.GetByIdAsync(id);
        if (user == null)
        {
            return TypedResults.Extensions.NoContent();
        }
        var userDto = new UserDTO()
        {
            Email = user.Email,
            Id = user.Id,
            Role = new UserDTO.UserRole()
            {
                Name = user.Role!.Name,
            },
        };
        return TypedResults.Ok(userDto);
    }
}
