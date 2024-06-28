

using System.Security.Claims;

namespace SimplifyCondoApi.Utils.ClaimsPrincipalExtension;

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
    ArgumentNullException.ThrowIfNull(id);
    return Guid.Parse(id);
  }
}