using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NextCondoApi.Entity;
using NextCondoApi.Services;
using NextCondoApi.Services.Auth;

namespace NextCondoApi;

public static class BuilderExtension
{
    public static void AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
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
                    "local"
                );
            defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
        });
    }

    public static void AddSwagger(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        var publicURL = configuration.GetSection("PUBLIC_URL").Get<string>();

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddServer(new OpenApiServer()
            {
                Url = "http://localhost:8080",
                Description = "Localhost"
            });
            if (!string.IsNullOrEmpty(publicURL))
            {
                c.AddServer(new OpenApiServer()
                {
                    Url = publicURL,
                    Description = "Public address"
                });
            }
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
        builder.Services.AddScoped<ICondominiumsRepository, CondominiumsRepository>();
        builder.Services.AddScoped<IAuthServiceHelper, AuthServiceHelper>();
        builder.Services.AddScoped<IAuthService, AuthService>();
    }
}
