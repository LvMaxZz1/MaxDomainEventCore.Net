using MaxDDDDemo.Domain.ValueObjectInterface;

namespace MaxDDDDemo.Domain.Entities;

public class Order : IEntity<Guid>
{
    public Guid Id { get; set; }

    public DateTime CreateOn { get; set; }

    public decimal TotalAmount { get; set; }
    
    public bool IsPaid { get; set; }

    public bool IsCancel { get; set; }

    public OrderAddress DeliveryAddress { get; set; } = null!;

    public Order(Guid id)
    {
        Id = id;
    }

    public void Create(OrderAddress orderAddress)
    {
        CreateOn = DateTime.Now;
        IsPaid = false;
        IsCancel = false;
        DeliveryAddress = orderAddress;
    }
}