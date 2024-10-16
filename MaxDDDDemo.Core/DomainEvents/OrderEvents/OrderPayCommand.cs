using MaxDDDDemo.Domain.Entities;
using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderPayCommand : IDomainCommand<OrderPayCommand>
{
    public Order Order { get; set; } = null!;

    public async Task Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        Console.WriteLine("OrderPayEvent 执行完毕");
    }
}