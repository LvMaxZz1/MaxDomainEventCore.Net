using System.Threading.Tasks;
using LvMaxDomainEventCore.Net.Initiator;

namespace LvMaxDomainEventCore.Net.DomainEvents;

public interface IDomainCommand<in T> : IDomainEvent where T : class, IDomainEvent
{
    Task Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken);
}