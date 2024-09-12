using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextCondoApi.Models.DTO;
using NextCondoApi.Services.Auth;
using System.Net.Mime;

namespace NextCondoApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService auth;

    public AuthController(IAuthService auth)
    {
        this.auth = auth;
    }

    [Authorize]
    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        Dictionary<string, string> claimsList = new();
        foreach (var claim in HttpContext.User.Claims)
        {
            claimsList.Add(claim.Type, claim.Value);
        }
        return Ok(claimsList.ToList());
    }

    [HttpPost("login")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(
        typeof(GenericResponseDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        MediaTypeNames.Application.ProblemJson)]
    public async Task<IActionResult> LoginAsync([FromForm] LoginCredentialsDTO credentials)
    {
        var result = await auth.LoginAsync(credentials.Email, credentials.Password, "local");
        if (result == false)
        {
            return Problem(
                    title: "Invalid credentials",
                    detail: "Invalid email or password",
                    statusCode: StatusCodes.Status401Unauthorized,
                    type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401"
                );
        }
        return Ok(new GenericResponseDTO() { Status = "Ok" });
    }

    [HttpPost("register")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(
        typeof(GenericResponseDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RegisterAsync([FromForm] RegisterUserDTO newUser)
    {
        var result = await auth
            .RegisterAsync(
                fullName: newUser.FullName,
                email: newUser.Email,
                password: newUser.Password,
                phone: newUser.Phone,
                scheme: "local"
            );
        if (result == false)
        {
            return Problem(
                        title: "Bad input",
                        detail: "Unable to register user with provided details",
                        statusCode: StatusCodes.Status400BadRequest,
                        type: "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400"
                    );
        }
        return Ok(new GenericResponseDTO() { Status = "Ok" });
    }

    [HttpGet("logout")]
    [Authorize]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType(
        typeof(GenericResponseDTO),
        StatusCodes.Status200OK,
        MediaTypeNames.Application.Json)]
    public async Task<IActionResult> SignOutAsync()
    {
        await auth.LogoutAsync("local");
        return Ok(new GenericResponseDTO() { Status = "Ok" });
    }
}
