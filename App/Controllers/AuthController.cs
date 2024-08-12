using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Utils.ClaimsPrincipalExtension;

namespace NextCondoApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
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
    var id = User.GetIdentity();
    return Ok(id);
  }
}
