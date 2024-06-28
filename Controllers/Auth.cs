using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplifyCondoApi.Utils.ClaimsPrincipalExtension;

namespace SimplifyCondoApi.Controllers
{
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class Auth : ControllerBase
  {

    [HttpGet]
    public IActionResult GetClaims()
    {
      Dictionary<string, string> claimsList = new();
      foreach (var claim in HttpContext.User.Claims)
      {
        claimsList.Add(claim.Type, claim.Value);
      }
      return Ok(claimsList.ToList());
    }

    [HttpGet("identity")]
    public IActionResult GetIdentity()
    {
      try
      {
        var id = User.GetIdentity();
        return Ok(id);
      }
      catch (Exception exception)
      {
        return BadRequest(exception);
      }

    }
  }
}
