using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Features.Validation;

namespace NextCondoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly NextCondoApiDbContext db;

        public StatusController(NextCondoApiDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            if (db.Database.CanConnect())
                return Ok();
            return Problem(
                title: "Connection error",
                detail: "Cannot connect to database",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "");
        }
    }
}
