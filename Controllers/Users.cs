using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplifyCondoApi.Controllers.UsersDTO;
using SimplifyCondoApi.Model;
using SimplifyCondoApi.Utils.ClaimsPrincipalExtension;

namespace SimplifyCondoApi.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class Users : ControllerBase
  {
    private readonly SimplifyCondoApiDbContext _context;

    public Users(SimplifyCondoApiDbContext context)
    {
      _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateUserDTO newUser)
    {
      try
      {
        var id = User.GetIdentity();
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        ArgumentNullException.ThrowIfNull(email);
        await _context.Users.AddAsync(new()
        {
          Id = id,
          Email = email
        });
        await _context.SaveChangesAsync();
        return Ok();
      }
      catch (Exception exception)
      {
        return BadRequest(exception.Message);
      }
    }

    [HttpGet]
    public IActionResult GetAll()
    {
      var users = _context.Users.Select(user => new UserDTO() { Id = user.Id, Email = user.Email });
      if (users.Any())
      {
        return Ok(users);
      }
      return NoContent();
    }
  }
}
