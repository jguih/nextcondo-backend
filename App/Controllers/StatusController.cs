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
        private readonly SimplifyCondoApiDbContext db;

        public StatusController(SimplifyCondoApiDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            if (db.Database.CanConnect())
                return Ok();
            return Problem(
                title: "Database connection error",
                detail: "Cannot connect to database",
                type: "connection",
                statusCode: StatusCodes.Status500InternalServerError
           );
        }
    }
}
