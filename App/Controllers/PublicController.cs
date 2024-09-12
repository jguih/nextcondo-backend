using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Entity;
using NextCondoApi.Models.DTO;
using System.Net.Mime;

namespace NextCondoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly NextCondoApiDbContext db;
        private readonly IConfiguration configuration;

        public PublicController(NextCondoApiDbContext db, IConfiguration configuration)
        {
            this.db = db;
            this.configuration = configuration;
        }

        [HttpGet("status")]
        [ProducesResponseType(
            typeof(GenericResponseDTO),
            StatusCodes.Status200OK,
            MediaTypeNames.Application.Json)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status500InternalServerError,
            MediaTypeNames.Application.ProblemJson)]
        public IActionResult GetStatus()
        {
            if (db.Database.CanConnect())
                return Ok(new GenericResponseDTO() { Status = "Ok" });
            return Problem(
                title: "Connection error",
                detail: "Cannot connect to database",
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500");
        }

        [HttpGet("publicURL")]
        [ProducesResponseType(
            typeof(PublicUrlDTO),
            StatusCodes.Status200OK,
            MediaTypeNames.Application.Json)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound,
            MediaTypeNames.Application.ProblemJson)]
        public ActionResult<string> GetPublicUrl()
        {
            var publicURL = configuration.GetSection("PUBLIC_URL").Get<string>();

            return publicURL == null ?
                Problem(
                    title: "Public URL not found",
                    detail: "Public URL not found",
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404") :
                Ok(new PublicUrlDTO(){ Url = publicURL });
        }
    }
}
