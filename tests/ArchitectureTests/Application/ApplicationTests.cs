using Application.Abstractions.Messaging;
using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests.Application;

public class ApplicationTests : BaseTest
{
    [Fact]
    public void CommandHandlers_Should_HaveCommandHandlerPostfix()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void QueryHandlers_Should_HaveQueryHandlerPostfix()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Commands_Should_BeSealed()
    {
        var result =
            Types.InAssembly(ApplicationAssembly)
                .That()
                .AreClasses()
                .And()
                .ImplementInterface(typeof(ICommand<>))
                .Should()
                .BeSealed()
                .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Queries_Should_BeSealed()
    {
        var result =
            Types.InAssembly(ApplicationAssembly)
                .That()
                .AreClasses()
                .And()
                .ImplementInterface(typeof(IQuery<>))
                .Should()
                .BeSealed()
                .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void CommandHandlers_Should_BeSealed()
    {
        var result =
            Types.InAssembly(ApplicationAssembly)
                .That()
                .AreClasses()
                .And()
                .ImplementInterface(typeof(ICommandHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void QueryHandlers_Should_BeSealed()
    {
        var result =
            Types.InAssembly(ApplicationAssembly)
                .That()
                .AreClasses()
                .And()
                .ImplementInterface(typeof(IQueryHandler<,>))
                .Should()
                .BeSealed()
                .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

}