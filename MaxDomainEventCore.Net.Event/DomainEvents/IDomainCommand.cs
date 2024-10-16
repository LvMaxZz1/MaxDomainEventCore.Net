namespace MaxDomainEventCore.Net.Event.DomainEvents;

public interface IDomainCommand<in T> : IDomainEvent where T : class, IDomainEvent
{
    Task Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken);
}