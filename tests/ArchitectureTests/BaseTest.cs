using System.Reflection;
using Application.Abstractions.Messaging;
using Domain.Entities.Abstract;
using Infrastructure.Persistence.Context;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IQuery<>).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(AppDbContext).Assembly;
}