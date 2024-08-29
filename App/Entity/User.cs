using System.Security.Claims;

namespace NextCondoApi.Entity;

public class User : BaseEntity
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string RoleId { get; set; }
    public required string FullName { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? Phone { get; set; }

    public required Role Role { get; set; }

    public List<Claim> GetClaims()
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, FullName),
            new(ClaimTypes.NameIdentifier, Id.ToString()),
            new(ClaimTypes.Email, Email),
            new(ClaimTypes.Role, Role.Name),
        };
        if (Phone != null)
        {
            claims.Add(new(ClaimTypes.MobilePhone, Phone));
        }
        return claims;
    }
}