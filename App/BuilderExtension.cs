using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using NextCondoApi.Entity;
using NextCondoApi.Features.Configuration;
using NextCondoApi.Services;
using NextCondoApi.Services.Auth;
using NextCondoApi.Services.SMTP;
using System.Threading.RateLimiting;

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
        builder.Services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();
        builder.Services.AddScoped<IAuthServiceHelper, AuthServiceHelper>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ISMTPService, SMTPService>();
    }

    public static void AddRateLimitingPolicies(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(_ => _
            .AddFixedWindowLimiter(policyName: "sendEmailVerificationCode", options =>
            {
                options.PermitLimit = 1;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 0;
            }));
    }

    public static void AddConfigurationOptions(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services
            .AddOptions<DbOptions>()
            .Bind(configuration.GetRequiredSection(DbOptions.DB))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<SMTPOptions>()
            .Bind(configuration.GetRequiredSection(SMTPOptions.SMTP))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<SystemOptions>()
            .Bind(configuration.GetRequiredSection(SystemOptions.SYSTEM))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
