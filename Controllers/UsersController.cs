using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Controllers.UsersControllerDTO;
using NextCondoApi.Entity;
using NextCondoApi.Utils.ClaimsPrincipalExtension;

namespace NextCondoApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly SimplifyCondoApiDbContext db;

    public UsersController(SimplifyCondoApiDbContext context)
    {
        db = context;
    }

    [HttpPut]
    public async Task<IActionResult> EditAsync([FromBody] EditUserDTO newUser)
    {
        var id = User.GetIdentity();
        var existing = await db.Users.FindAsync(id);

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

        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = db.Users
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

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error-development")]
    public IActionResult HandleErrorDevelopment(
        [FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }

        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        return Problem(
            detail: exceptionHandlerFeature.Error.StackTrace,
            title: exceptionHandlerFeature.Error.Message);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError() =>
        Problem();
}
