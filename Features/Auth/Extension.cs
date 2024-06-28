
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SimplifyCondoApi.Auth;

public static class AuthServices
{
  public static void AddAuth(this WebApplicationBuilder builder)
  {
    var JwtSecret = builder.Configuration.GetRequiredSection("Supabase:JwtSecret").Get<string>()!;
    var Audiences = builder.Configuration.GetRequiredSection("Supabase:Audiences").Get<List<string>>()!;
    var Issuer = builder.Configuration.GetRequiredSection("Supabase:Issuer").Get<string>()!;

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication().AddJwtBearer(o =>
    {
      o.TokenValidationParameters = new()
      {
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret)),
        ValidAudiences = Audiences,
        ValidIssuer = Issuer
      };
    });
  }
}