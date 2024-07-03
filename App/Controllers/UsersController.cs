using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplifyCondoApi.Controllers.UsersControllerDTO;
using SimplifyCondoApi.Entity;
using SimplifyCondoApi.Utils.ClaimsPrincipalExtension;

namespace SimplifyCondoApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
  private readonly SimplifyCondoApiDbContext _context;

  public UsersController(SimplifyCondoApiDbContext context)
  {
    _context = context;
  }

  [HttpPost]
  public async Task<IActionResult> Register([FromBody] CreateUserDTO newUser)
  {
    var id = User.GetIdentity();
    var existing = await _context.Users.FindAsync(id);

    if (existing != null)
    {
      return Ok();
    }

    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    var role = await _context.Roles.FindAsync("Tenant");
    ArgumentNullException.ThrowIfNull(email);
    ArgumentNullException.ThrowIfNull(role);

    await _context.Users.AddAsync(new()
    {
      Id = id,
      Email = email,
      RoleId = role.Name,
    });
    await _context.SaveChangesAsync();

    return Ok();
  }

  [HttpGet]
  public IActionResult GetAll()
  {
    var users = _context.Users
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
