namespace MaxDDDDemo.Domain.ValueObjectInterface;

public class OrderAddress : IEntity<Guid>
{
    private OrderAddress(
        Guid orderId, string city, string street, string houseNumber, string postCode)
    {
        SetOrderId(orderId);
        SetCity(city);
        SetStreet(street);
        SetHouseNumber(houseNumber);
        SetPostCode(postCode);
    }

    public Guid OrderId { get; private set; } = Guid.Empty;

    public string City { get; private set; } = null!;

    public string Street { get; private set; } = null!;

    public string HouseNumber { get; private set; } = null!;

    public string PostCode { get; private set; } = null!;

    public static OrderAddress Create(
        Guid orderId, string city, string street, string houseNumber, string postCode)
    {
        return new OrderAddress(orderId, city, street, houseNumber, postCode);
    }

    private void SetCity(string city)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(city);

        City = city;
    }

    private void SetStreet(string street)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);


        Street = street;
    }

    private void SetHouseNumber(string houseNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(houseNumber);

        HouseNumber = houseNumber;
    }

    private void SetPostCode(string postCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(postCode);


        PostCode = postCode;
    }

    private void SetOrderId(Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("orderId cannot be an empty Guid.", nameof(orderId));
        }

        OrderId = orderId;
    }

    public Guid Id { get; }
}