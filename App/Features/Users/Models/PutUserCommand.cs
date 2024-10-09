
using System.ComponentModel.DataAnnotations;

namespace NextCondoApi.Features.UsersFeature.Models;

public class PutUserCommand
{
    [MaxLength(255)]
    [Required]
    public required string FullName { get; set; }
    [Phone]
    [MaxLength(30)]
    public string? Phone { get; set; }
}