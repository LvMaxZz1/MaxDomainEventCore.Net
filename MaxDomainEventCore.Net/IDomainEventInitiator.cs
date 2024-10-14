using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net;

public interface IDomainEventInitiator
{
    Task PublishAsync<T>(T @event) where T : class, IDomainCommand<T>;
    Task<TR> RequestAsync<T, TR>(T @event) where T : class, IDomainEvent where TR : class, IDomainResponse;
}