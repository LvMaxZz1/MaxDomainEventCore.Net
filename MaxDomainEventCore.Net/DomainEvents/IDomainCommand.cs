using System.Threading.Tasks;
using MaxDomainEventCore.Net.Initiator;

namespace MaxDomainEventCore.Net.DomainEvents;

public interface IDomainCommand<in T> : IDomainEvent where T : class, IDomainEvent
{
    Task Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken);
}