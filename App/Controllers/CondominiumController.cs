using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NextCondoApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class CondominiumController : ControllerBase
{

}
