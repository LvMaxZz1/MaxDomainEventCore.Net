using MaxDDDDemo.Domain.Entities;
using MaxDomainEventCore.Net;
using MaxDomainEventCore.Net.DomainEvents;
using MaxDomainEventCore.Net.Initiator;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderPayCommand : IDomainCommand<OrderPayCommand>
{
    public Order Order { get; set; } = null!;

    public async Task Run(IDomainEventInitiator domainEventInitiator)
    {
        Console.WriteLine("OrderPayEvent 执行完毕");
    }
}