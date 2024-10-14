using DDDDemo.Core.Dtos;
using MaxDomainEventCore.Net;
using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderGetRequest : IDomainRequest<OrderGetRequest, OrderDto>
{
    public Guid OrderId { get; set; }

    public Task<OrderDto> Run(IDomainEventInitiator domainEventInitiator)
    {
        Console.WriteLine("OrderCancelEvent 执行完毕");
        return Task.FromResult(new OrderDto{ Id = OrderId});
    }
}