using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Api.FunctionalTests.Abstractions;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("dzik")
        .WithUsername("dzikuuu")
        .WithPassword("dzikuuu123")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString(), e =>
                    e.MigrationsAssembly("Migrators.PostgreSQL"));
            });
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}
