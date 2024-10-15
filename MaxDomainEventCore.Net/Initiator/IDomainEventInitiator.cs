using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net.Initiator;

public interface IDomainEventInitiator
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IDomainCommand<T>;
    
    Task<TR> RequestAsync<T, TR>(T @event, CancellationToken cancellationToken = default) where T : class, IDomainEvent where TR : class, IDomainResponse;
}