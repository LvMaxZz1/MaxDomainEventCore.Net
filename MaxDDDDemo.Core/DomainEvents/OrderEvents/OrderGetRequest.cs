using DDDDemo.Core.Dtos;
using MaxDomainEventCore.Net.DomainEvents;
using MaxDomainEventCore.Net.Initiator;

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