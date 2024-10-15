using LvMaxDomainEventCore.Net.DomainEvents;

namespace DDDDemo.Core.Dtos;

public class OrderDto : IDomainResponse
{
    public Guid Id { get; set; }
}