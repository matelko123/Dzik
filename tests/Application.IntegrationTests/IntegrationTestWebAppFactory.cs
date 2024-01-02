using Host;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Application.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("dzik")
        .WithUsername("dziku")
        .WithPassword("dziku123")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<BaseDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<BaseDbContext>(opt =>
            {
                opt
                    .UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    public Task InitializeAsync()
        => _dbContainer.StartAsync();

    public new Task DisposeAsync()
        => _dbContainer.StopAsync();
}