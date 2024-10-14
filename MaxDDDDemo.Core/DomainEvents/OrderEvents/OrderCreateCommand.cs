using MaxDDDDemo.Domain.Entities;
using MaxDDDDemo.Domain.ValueObjectInterface;
using MaxDomainEventCore.Net;
using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderCreateCommand : IDomainCommand<OrderCreateCommand>
{
    public Order Order { get; set; } = null!;

    public async Task Run(IDomainEventInitiator domainEventInitiator)
    {
        Order.Create(OrderAddress.Create(Order.Id, "武汉", "汉正街", "广益天下", "50050"));
        Console.WriteLine("OrderCreateEvent 执行完毕");
    }
}