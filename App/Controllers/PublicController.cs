using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NextCondoApi.Entity;
using NextCondoApi.Features.Configuration;
using NextCondoApi.Features.PublicFeature.Models;
using System.Net.Mime;

namespace NextCondoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly NextCondoApiDbContext _db;
        private readonly IOptions<SystemOptions> _systemOptions;

        public PublicController(NextCondoApiDbContext db, IOptions<SystemOptions> systemOptions)
        {
            _db = db;
            _systemOptions = systemOptions;
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
            if (_db.Database.CanConnect())
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
            var publicURL = _systemOptions.Value.PUBLIC_URL;

            return publicURL == null ?
                Problem(
                    title: "Public URL not found",
                    detail: "Public URL not found",
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404") :
                Ok(new PublicUrlDTO() { Url = publicURL });
        }
    }
}
