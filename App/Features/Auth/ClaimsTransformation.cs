using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using NextCondoApi.Features.Validation;
using NextCondoApi.Entity;
using NextCondoApi.Utils.ClaimsPrincipalExtension;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace NextCondoApi.Auth;

public class AuthClaimsTransformation : IClaimsTransformation
{
    private readonly NextCondoApiDbContext db;

    public AuthClaimsTransformation(NextCondoApiDbContext context)
    {
        db = context;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();
        var claimType = "internal_role";
        var user_id = principal.GetIdentity();
        var user = await db.Users.FindAsync(user_id);

        if (user == null)
        {
            var defaultRole = await db.GetDefaultRole();
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value ?? "";

            if (email == null)
            {
                throw new HttpResponseException(StatusCodes.Status401Unauthorized, "Could not find user email address in provided claims.");
            }

            await db.Users.AddAsync(new User { 
                Id = user_id, 
                RoleId = defaultRole.Name, 
                Email = email, 
                FullName = name 
            });

            await db.SaveChangesAsync();
            user = await db.Users.FindAsync(user_id);
        }

        if (user == null)
        {
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, "Failed to retrieve or create user from database.");
        }

        if (!principal.HasClaim(claim => claim.Type == claimType))
        {
            claimsIdentity.AddClaim(new Claim(claimType, user.RoleId));
        }

        principal.AddIdentity(claimsIdentity);
        return await Task.FromResult(principal);
    }
}