
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NextCondoApi.Features.Validation;
using NextCondoApi.Auth;
using NextCondoApi.Entity;
using NextCondoApi.Swagger;

namespace NextCondoApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddDbContext<SimplifyCondoApiDbContext>(opt =>
          opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        });

        builder.AddAuth();
        builder.AddSwagger();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IClaimsTransformation, AuthClaimsTransformation>();
        builder.Services.ConfigureHttpJsonOptions(opt =>
        {
            opt.SerializerOptions.IncludeFields = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error-dev");
            app.UseForwardedHeaders();
            app.UseSwagger();
            app.UseSwaggerUI();
        } else
        {
            app.UseExceptionHandler("/error");
            app.UseForwardedHeaders();
        }

        app.MapSwagger().RequireAuthorization();
        // app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
