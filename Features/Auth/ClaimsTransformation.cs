using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace SimplifyCondoApi.Auth;

public class AuthClaimsTransformation : IClaimsTransformation
{
  // private readonly SimplifyCondoApiDbContext _context;

  // public AuthClaimsTransformation(SimplifyCondoApiDbContext context)
  // {
  //   _context = context;
  // }

  public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
  {
    ClaimsIdentity claimsIdentity = new ClaimsIdentity();
    var claimType = "internal_role";

    if (!principal.HasClaim(claim => claim.Type == claimType))
    {
      claimsIdentity.AddClaim(new Claim(claimType, "testing role"));
    }

    principal.AddIdentity(claimsIdentity);
    return Task.FromResult(principal);
  }
}