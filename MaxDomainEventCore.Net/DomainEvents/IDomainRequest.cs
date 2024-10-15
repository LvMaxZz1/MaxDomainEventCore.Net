using System.Threading.Tasks;
using MaxDomainEventCore.Net.Initiator;

namespace MaxDomainEventCore.Net.DomainEvents;

public interface IDomainRequest<in T, TR> : IDomainEvent
    where T : class, IDomainEvent
    where TR : class, IDomainResponse
{
    Task<TR> Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken);
}