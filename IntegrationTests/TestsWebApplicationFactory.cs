﻿using IntegrationTests.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NextCondoApi;
using Microsoft.Extensions.Options;
using NextCondoApi.Features.Configuration;

namespace IntegrationTests;

public class TestsWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {

        });

        builder.ConfigureTestServices(services =>
        {

        });

        builder.UseEnvironment("Tests");
    }

    public override async ValueTask DisposeAsync()
    {
        // Clear Resources
        using (var scope = Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            var configuration = provider.GetRequiredService<IOptions<DbOptions>>();
            await DbUtils.CleanUpAsync(configuration);
        };
        GC.SuppressFinalize(this);
        await base.DisposeAsync();
    }
}
