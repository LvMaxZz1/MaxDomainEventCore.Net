using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDDDDemo.Core.Dtos;

public class OrderDto : IDomainResponse
{
    public Guid Id { get; set; }
}