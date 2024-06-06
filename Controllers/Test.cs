using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class Test : ControllerBase
  {

    [HttpGet]
    public IActionResult Get()
    {
      Dictionary<string, string> claimsList = new();
      foreach (var claim in HttpContext.User.Claims)
      {
        claimsList.Add(claim.Type, claim.Value);
      }
      return Ok(claimsList.ToList());
    }
  }
}
