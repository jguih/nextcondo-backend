
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SimplifyCondoApi.Auth;
using SimplifyCondoApi.Model;
using SimplifyCondoApi.Swagger;

namespace simplify_condo_api;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<SimplifyCondoApiDbContext>(opt =>
      opt.UseInMemoryDatabase("test")
    );

    builder.Services.AddControllers();
    builder.AddAuth();
    builder.AddSwagger();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddTransient<IClaimsTransformation, AuthClaimsTransformation>();

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
