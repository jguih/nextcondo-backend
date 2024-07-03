
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SimplifyCondoApi.Auth;
using SimplifyCondoApi.Entity;
using SimplifyCondoApi.Swagger;

namespace simplify_condo_api;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Services.AddDbContext<SimplifyCondoApiDbContext>(opt =>
      opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
    );

    builder.Services.AddControllers();
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
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.MapSwagger().RequireAuthorization();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
  }
}
