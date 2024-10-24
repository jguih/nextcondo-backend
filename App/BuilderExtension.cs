using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using NextCondoApi.Entity;
using NextCondoApi.Features.AuthFeature.Services;
using NextCondoApi.Features.CommonAreasFeature.Services;
using NextCondoApi.Features.CondominiumFeature.Services;
using NextCondoApi.Features.Configuration;
using NextCondoApi.Features.OccurrencesFeature.Services;
using NextCondoApi.Features.TenantsFeature.Services;
using NextCondoApi.Features.UsersFeature.Services;
using NextCondoApi.Services;
using NextCondoApi.Services.SMTP;
using System.Net.Mime;
using System.Text.Json;
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
                    context.Response.WriteAsJsonAsync(
                        new ProblemDetails()
                        {
                            Title = "Unauthorized",
                            Status = StatusCodes.Status401Unauthorized,
                            Detail = "Not authorized to access requested resource.",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401"
                        },
                        JsonSerializerOptions.Default,
                        MediaTypeNames.Application.ProblemJson);
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
        SystemOptions systemOptions = new();
        configuration.GetRequiredSection(SystemOptions.SYSTEM).Bind(systemOptions);

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddServer(new OpenApiServer()
            {
                Url = "http://localhost:8080",
                Description = "Localhost"
            });
            if (!string.IsNullOrEmpty(systemOptions.PUBLIC_URL))
            {
                c.AddServer(new OpenApiServer()
                {
                    Url = systemOptions.PUBLIC_URL,
                    Description = "Public address"
                });
            }
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
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
        builder.Services.AddScoped<IRolesRepository, RolesRepository>();
        builder.Services.AddScoped<ICondominiumsRepository, CondominiumsRepository>();
        builder.Services.AddScoped<IEmailVerificationCodeRepository, EmailVerificationCodeRepository>();
        builder.Services.AddScoped<ICurrentCondominiumRepository, CurrentCondominiumRepository>();
        builder.Services.AddScoped<IOccurrencesRepository, OccurrencesRepository>();
        builder.Services.AddScoped<IOccurrenceTypesRepository, OccurrenceTypesRepository>();
        builder.Services.AddScoped<ICondominiumUserRepository, CondominiumUserRepository>();
        builder.Services.AddScoped<ICommonAreasRepository, CommonAreasRepository>();
        builder.Services.AddScoped<ICommonAreaReservationsRepository, CommonAreaReservationsRepository>();
        builder.Services.AddScoped<ICommonAreaTypesRepository, CommonAreaTypesRepository>();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddDataProtection();
        builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        builder.Services.AddScoped<IAuthServiceHelper, AuthServiceHelper>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ISMTPService, SMTPService>();
        builder.Services.AddScoped<ICondominiumService, CondominiumService>();
        builder.Services.AddScoped(typeof(OccurrencesService));
        builder.Services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        builder.Services.AddScoped<ITenantsService, TenantsService>();
        builder.Services.AddScoped<ICommonAreasService, CommonAreasService>();
        builder.Services.AddScoped<IBookingSlotService, BookingSlotService>();
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
