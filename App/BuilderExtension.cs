using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        builder.Services.AddAuthentication()
            .AddJwtBearer("supabase", o =>
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
                })
            .AddCookie("local", options =>
            {
                options.LoginPath = string.Empty;
                options.LogoutPath = string.Empty;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.WriteAsJsonAsync(new ProblemDetails()
                    {
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Not authorized to access requested resource.",
                        Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401"
                    });
                    return Task.CompletedTask;
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    "supabase",
                    "local"
                );
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
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
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddDataProtection();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IRolesRepository, RolesRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
    }
}
