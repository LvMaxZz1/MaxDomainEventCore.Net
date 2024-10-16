using DDDDemo.Controllers;
using MaxDDDDemo.Core.DomainEvents.OrderEvents;
using MaxDDDDemo.Core.Dtos;
using MaxDDDDemo.Domain.Entities;
using MaxDomainEventCore.Net.Event.DomainEvents;
using Microsoft.AspNetCore.Mvc;

namespace MaxDDDDemo.Api.Controllers;

[Route("Order")]
public class OrderController : BaseController
{
    private readonly IDomainEventInitiator _domainEventInitiator;

    public OrderController(IDomainEventInitiator domainEventInitiator)
    {
        _domainEventInitiator = domainEventInitiator;
    }

    [HttpPost]
    [ProducesResponseType<OrderDto>(200)]
    public async Task<IActionResult> CreateOrder()
    {
        await _domainEventInitiator.PublishAsync(new OrderCreateCommand
        {
            Id = Guid.NewGuid(),
            Order = new Order(Guid.NewGuid()),
            Name = "LvMaxZz",
            Age = 24,
            Price = 112,
            TotalAmount = 1231232131,
            obj = null,
            list = ["1","2"],
            list2 = [1,2,3],
            list3 = [123123,3123123,123123],
            list4 = [1123.11m,31312.22m,413412.33m],
            list5 = [null,null],
            list6 = [new Order(Guid.NewGuid()), new Order(Guid.NewGuid())],
            list7 = [Guid.NewGuid(),Guid.NewGuid()],
            Status =  OrderStatus.Created

        });

        return Ok();
    }
    
    [HttpGet]
    [ProducesResponseType<OrderDto>(200)]
    public async Task<IActionResult> GetOrder()
    {
        var dto = await _domainEventInitiator.SendAsync<OrderGetRequest, OrderDto>(new OrderGetRequest { OrderId = Guid.NewGuid() });
        return Ok(dto);
    }
}