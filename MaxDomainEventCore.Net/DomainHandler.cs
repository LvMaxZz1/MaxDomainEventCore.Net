using System.Threading.Tasks;
using MaxDomainEventCore.Net.Dependency;
using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net;

public class DomainHandler : IMaxScopeDependency
{
    public async Task Handle<T>(T @event, IDomainEventInitiator domainEventInitiator) where T : class, IDomainCommand<T>
    {
        await @event.Run(domainEventInitiator);
    }

    public async Task<TR> Handle<T, TR>(T @event, IDomainEventInitiator domainEventInitiator)
        where T : class, IDomainRequest<T, TR>
        where TR : class, IDomainResponse
    {
        var response = await @event.Run(@event, domainEventInitiator);
        return response;
    }
}