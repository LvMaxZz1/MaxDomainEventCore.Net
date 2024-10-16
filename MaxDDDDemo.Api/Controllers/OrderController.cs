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
            Order = new Order(Guid.NewGuid())
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