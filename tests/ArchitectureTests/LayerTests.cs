using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests;

public class LayerTests : BaseTest
{
    [Fact]
    public void Domain_Should_NotHaveDependency()
    {
        // Arrange
        var otherProjects = new[]
        {
            ApplicationAssembly,
            InfrastructureAssembly,
            HostAssembly
        }
        .Select(x => x.GetName().Name)
        .ToArray();

        // Act
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(otherProjects)
            .GetResult()
            .IsSuccessful;

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_NotDependOnInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult()
            .IsSuccessful;

        result.Should().BeTrue();
    }
}