namespace MaxDomainEventCore.Net.Event.DomainEvents;

public interface IDomainRequest<in T, TR> : IDomainEvent
    where T : class, IDomainEvent
    where TR : class, IDomainResponse
{
    Task<TR> Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken);
}