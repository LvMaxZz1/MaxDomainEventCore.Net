using DDDDemo.Core.Dtos;
using MaxDDDDemo.Domain.Entities;
using MaxDDDDemo.Domain.ValueObjectInterface;
using LvMaxDomainEventCore.Net.DomainEvents;
using LvMaxDomainEventCore.Net.Initiator;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderCreateCommand : IDomainCommand<OrderCreateCommand>
{
    public Order Order { get; set; } = null!;

    public async Task Run(IDomainEventInitiator domainEventInitiator)
    {
        Order.Create(OrderAddress.Create(Order.Id, "武汉", "汉正街", "广益天下", "50050"));
        Console.WriteLine("OrderCreateEvent 执行完毕");
        var a = await domainEventInitiator.RequestAsync<OrderGetRequest, OrderDto>(new OrderGetRequest());
    }
}