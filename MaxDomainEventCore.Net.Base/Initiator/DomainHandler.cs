using MaxDomainEventCore.Net.Base.Dependency;
using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Base.Initiator;

public class DomainHandler : IMaxScopeDependency
{
    public async Task Handle<T>(T @event, IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken) where T : class, IDomainCommand<T>
    {
        await @event.Run(domainEventInitiator, cancellationToken);
    }

    public async Task<TR> Handle<T, TR>(T @event, IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
        where T : class, IDomainRequest<T, TR>
        where TR : class, IDomainResponse
    {
        var response = await @event.Run(domainEventInitiator, cancellationToken);
        return response;
    }
}