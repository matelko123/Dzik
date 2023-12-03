namespace Domain.Entities.Abstract;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public Guid Id { get; init; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public List<IDomainEvent> DomainEvents => _domainEvents;

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}