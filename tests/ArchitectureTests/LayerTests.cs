using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class LayerTests : BaseTest
{
    [Fact]
    public void Domain_Should_NotHaveAnyDependency()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_DependOnDomain()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .HaveDependencyOn("Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Infrastructure_Should_DependOnDomainAndApplication()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .HaveDependencyOnAll("Domain", "Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}