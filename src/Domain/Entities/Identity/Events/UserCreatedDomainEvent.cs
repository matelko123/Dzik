using Domain.Entities.Abstract;

namespace Domain.Entities.Identity.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;