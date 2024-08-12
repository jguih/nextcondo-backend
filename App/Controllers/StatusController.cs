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
        public string GetStatus()
        {
            if (db.Database.CanConnect())
                return "Working";
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, "Could not stablish connection with database");
        }
    }
}
