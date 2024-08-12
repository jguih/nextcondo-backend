using NextCondoApi.Features.Validation;
using System.Security.Claims;

namespace NextCondoApi.Utils.ClaimsPrincipalExtension;

public static class ClaimsPrincipalExtension
{
    /// <summary>
    /// Get user identity from ClaimsPrincipal
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FormatException" />
    public static Guid GetIdentity(this ClaimsPrincipal claimsPrincipal)
    {
        var id = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (id == null)
        {
            throw new HttpResponseException(StatusCodes.Status401Unauthorized, "Could dot determine user identity");
        }
        return Guid.Parse(id);
    }
}