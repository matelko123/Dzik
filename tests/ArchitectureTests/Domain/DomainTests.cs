using System.Reflection;
using Domain.Entities.Abstract;
using FluentAssertions;
using Mono.Cecil;
using NetArchTest.Rules;

namespace ArchitectureTests.Domain;

public class DomainTests : BaseTest
{
    [Fact]
    public void Entities_Should_BeSealed()
    {
        TestResult? result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        TestResult? result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_Should_HaveDomainEventPostfix()
    {
        TestResult? result = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_Should_HavePrivateParameterlessConstructor()
    {
        IEnumerable<Type>? entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        List<Type> failingTypes = [];
        // failingTypes.AddRange(from entityType in entityTypes let constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance) where constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0) select entityType);

        foreach (Type? entityType in entityTypes)
        {
            ConstructorInfo[] constructors = entityType.GetConstructors(BindingFlags.NonPublic |
                                                                        BindingFlags.Instance);

            if (constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                failingTypes.Add(entityType);
            }
        }
        
        failingTypes.Should().BeEmpty();
    }

    [Fact]
    public void Entities_Should_BeEncapsulated()
    {
        var result =
            Types.InAssembly(DomainAssembly)
                .That()
                .AreClasses()
                .And()
                .Inherit(typeof(Entity))
                .Should()
                .MeetCustomRule(new EncapsulationRule())
                .GetResult();
        
        result.IsSuccessful.Should().BeTrue();
    }
}

public class EncapsulationRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        return TypeShouldNotHavePublicSetters(type) && TypeShouldNotHavePublicSetters(type.BaseType?.Resolve());
    }
    
    private static bool TypeShouldNotHavePublicSetters(TypeDefinition? type)
    {
        return type?.Properties
                   .All(x => x.SetMethod is null // For IReadOnlyList
                             || !x.SetMethod.IsPublic // Allow Private and Protected
                             || x.SetMethod.ReturnType.FullName.Contains("IsExternalInit")) // For C# 9 init
               ?? false;
    }
}