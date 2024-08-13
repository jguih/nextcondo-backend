using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using System.Text;

namespace NextCondoApi;

public static class BuilderExtension
{
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        var JwtSecret = builder.Configuration.GetSection("JWT_SECRET").Get<string>()!;
        var Audiences = builder.Configuration.GetSection("JWT_AUDIENCES").Get<string>()!;
        var Issuer = builder.Configuration.GetSection("JWT_ISSUER").Get<string>()!;

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication().AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret)),
                ValidAudiences = Audiences.Split(","),
                ValidIssuer = Issuer
            };
        });
    }

    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
          {
              new OpenApiSecurityScheme
              {
                  Reference = new OpenApiReference
                  {
                      Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  },
                  Scheme = "oauth2",
                  Name = "Bearer",
                  In = ParameterLocation.Header
              },
              new List<string>()
          }
      });
        });
    }

    public static void ConfigureForwardedHeaders(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
    }
}
