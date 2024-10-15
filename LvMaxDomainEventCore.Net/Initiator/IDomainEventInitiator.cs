using LvMaxDomainEventCore.Net.DomainEvents;

namespace LvMaxDomainEventCore.Net.Initiator;

public interface IDomainEventInitiator
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IDomainCommand<T>;
    
    Task<TR> RequestAsync<T, TR>(T @event, CancellationToken cancellationToken = default) where T : class, IDomainEvent where TR : class, IDomainResponse;

    Task<TR> SendAsync<T, TR>(T @event, CancellationToken cancellationToken = default)
        where T : class, IDomainEvent where TR : class, IDomainResponse;
}