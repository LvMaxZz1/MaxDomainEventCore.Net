using MaxDDDDemo.Core.Dtos;
using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderGetRequest : IDomainRequest<OrderGetRequest, OrderDto>
{
    public Guid OrderId { get; set; }

    public Task<OrderDto> Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        Console.WriteLine("OrderGetRequest 执行完毕");
        return Task.FromResult(new OrderDto{ Id = OrderId});
    }
}