
using NextCondoApi.Features.Validation;
using NextCondoApi.Entity;

namespace NextCondoApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddDbContext<NextCondoApiDbContext>();
        builder.ConfigureForwardedHeaders();
        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.AddAuth();
        builder.AddSwagger(configuration);
        builder.AddRepositories();
        builder.AddServices();
        builder.AddRateLimitingPolicies();
        builder.AddConfigurationOptions(configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureHttpJsonOptions(opt =>
        {
            opt.SerializerOptions.IncludeFields = true;
        });
        builder.Services.AddProblemDetails();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseForwardedHeaders();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseForwardedHeaders();
        }

        app.MapSwagger().RequireAuthorization();
        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();
        app.Run();
    }
}
