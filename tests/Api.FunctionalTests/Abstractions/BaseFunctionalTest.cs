using Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Api.FunctionalTests.Abstractions;

public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();

        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected HttpClient HttpClient { get; set; }
    private readonly IServiceScope _scope;
    protected readonly AppDbContext DbContext;
}
