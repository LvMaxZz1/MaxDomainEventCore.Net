using MaxDDDDemo.Core.Dtos;
using MaxDDDDemo.Domain.Entities;
using MaxDDDDemo.Domain.ValueObjectInterface;
using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDDDDemo.Core.DomainEvents.OrderEvents;

public class OrderCreateCommand : IDomainCommand<OrderCreateCommand>
{
    public Guid Id { get; set; }
    public Order Order { get; set; } = null!;

    public string? Name { get; set; } = "张三";
    
    public int Age { get; set; }
    
    public long Price { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public object obj { get; set; }
    
    public List<string> list { get; set; }
    
    public List<int> list2 { get; set; }
    
    public List<long> list3 { get; set; }
    
    public List<decimal>? list4 { get; set; }
    
    public List<object> list5 { get; set; }
    
    public List<Order> list6 { get; set; }
    
    public List<Guid> list7 { get; set; }

    private IDomainEventInitiator _domainEventInitiator { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Cancel;


    public async Task Run(IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        Order.Create(OrderAddress.Create(Order.Id, "武汉", "汉正街", "广益天下", "50050"));
        Console.WriteLine("OrderCreateEvent 执行完毕");
        var a = await domainEventInitiator.RequestAsync<OrderGetRequest, OrderDto>(new OrderGetRequest(), cancellationToken);
    }
}

public enum OrderStatus
{
    Created,
    Paid,
    Cancel
}