using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Util;

public class IntegrationTestFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ChirpDBContext>));
            
            services.AddDbContext<ChirpDBContext>(options => options.UseSqlite("Filename=:memory:"));
            services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ChirpDBContext>();
            
            var dbContext = CreateDbContext(services);
            dbContext.Database.EnsureCreated();
        });
        
        // builder.ConfigureServices(services =>
        // {
        //     services.RemoveAll(typeof(DbContextOptions<ChirpDBContext>));
        //     
        //     services.AddDbContext<ChirpDBContext>(options => options.UseSqlite("Filename=:memory:"));
        //     services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = false)
        //         .AddEntityFrameworkStores<ChirpDBContext>();
        //     
        //     var dbContext = CreateDbContext(services);
        //     dbContext.Database.EnsureCreated();
        // });
    }

    private static ChirpDBContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
        
        return dbContext;
    }
}