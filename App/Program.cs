
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using NextCondoApi.Features.Validation;
using NextCondoApi.Auth;
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
        builder.AddSwagger();
        builder.AddRepositories();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IClaimsTransformation, AuthClaimsTransformation>();
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
        } else
        {
            app.UseForwardedHeaders();
        }

        app.MapSwagger().RequireAuthorization();
        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
