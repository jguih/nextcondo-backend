using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NextCondoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public string GetStatus()
        {
            return "Working";
        }
    }
}
