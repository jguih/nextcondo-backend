using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.AuthFeature.Models;

public class RegisterUserDTO
{
    [MaxLength(255)]
    [Required]
    public required string FullName { get; set; }
    [EmailAddress]
    [Required]
    public required string Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
    [MaxLength(30)]
    [MinLength(8)]
    [Required]
    public required string Password { get; set; }
}

public class LoginCredentialsDTO
{
    [EmailAddress]
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}