# MaxDomainEventCore.Net
## 此包依赖于Autofac，它有助于实现中介和域事件模式，使您能够专注于逻辑实现，而无需处理事件订阅和服务注册。(后续会更新通过微软原生容器实现的包)
## This package relies on Autofac, which helps implement mediation and domain event patterns, allowing you to focus on logical implementation without dealing with event subscriptions and service registration.(The package implemented through Microsoft native containers will be updated in the future)

## 创建领域事件（Create Domain Event）
### 1.1 创建无响应领域事件（Create No Response Domain Event）

```csharp
public class OrderCreateCommand : IDomainCommand<OrderCreateCommand>
{
    public Order Order { get; set; } = null!;

    public async Task Run(
        IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        //Your logic
    }
}
```

### 1.2 创建有响应领域事件（Create Has Response Domain Event）
```csharp
public class OrderGetRequest : IDomainRequest<OrderGetRequest, OrderDto>
{
    public Guid OrderId { get; set; }
    
    public async Task<OrderDto> Run(
        IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        //Your logic
    }
}
```

### 1.3 领域事件中的依赖注入（Dependency injection in domain events）
After creating a property, whether it is private or public, property injection will be automatically performed
创建一个属性后，无论是私有/公开的，都会自动进行属性注入
```csharp
public class OrderGetRequest : IDomainRequest<OrderGetRequest, OrderDto>
{
    public Guid OrderId { get; set; }

    private IService Service { get; set; } 

    public IDbContext DbContext { get; set;}
    
    public async Task<OrderDto> Run(
        IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        //Your logic
    }
}
```

### 1.4 领域事件中的发起领域事件（Initiating Domain Events in Domain Events）
```csharp
public class OrderGetRequest : IDomainRequest<OrderGetRequest, OrderDto>
{
    public Guid OrderId { get; set; }

    private IService Service { get; set; } 
    
    public async Task<OrderDto> Run(
        IDomainEventInitiator domainEventInitiator, CancellationToken cancellationToken)
    {
        //Your logic
        await domainEventInitiator.CommandAsync(new OrderCreateCommand());
    }
}
```

## 触发你的领域事件（Trigger Domain Events）
### 1.1 发布无响应领域事件（Publish Not Response DomainEvent）
```csharp
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
```

### 1.2 请求有响应领域事件（Request Has Response DomainEvent）
``` csharp
    [HttpGet]
    [ProducesResponseType<OrderDto>(200)]
    public async Task<IActionResult> GetOrder()
    {
        var dto = await _domainEventInitiator.RequestAsync<OrderGetRequest, OrderDto>(new OrderGetRequest { OrderId = Guid.NewGuid() });
        return Ok(dto);
    }
```

### 1.3 发送有相应的领域命令（Send Has Response DomainEvent）
```csharp
    [HttpGet]
    [ProducesResponseType<OrderDto>(200)]
    public async Task<IActionResult> GetOrder()
    {
        var dto = await _domainEventInitiator.SendAsync<OrderGetCommand, OrderDto>(new OrderGetCommand { OrderId = Guid.NewGuid() });
        return Ok(dto);
    }
```

## 注册MaxDomainEventCore(Register MaxDomainEventCore)

### 1.1注册IDomainEventInitiator(Register IDomainEventInitiator)
```csharp
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder =>
{
    containerBuilder.RegisterMaxDomainEventInitiator();
}));
```

### 1.2使用IDomainEventInitiator(Using IDomainEventInitiator)
```csharp
[Route("Order")]
public class OrderController : BaseController
{
    private readonly IDomainEventInitiator _domainEventInitiator;

    public OrderController(IDomainEventInitiator domainEventInitiator)
    {
        _domainEventInitiator = domainEventInitiator;
    }
}
```

## 领域事件拦截器（Domain Event Interceptor）

### 1.1 注册领域事件拦截器（Register Max Domain Event Interceptor）
```csharp
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder =>
{
    //通过Autofac注入服务
    //Inject services through Autofac
    containerBuilder.RegisterMaxDomainEventInterceptor();
}));

app.InitializeMaxDomainEventInterceptor();
```

### 1.2 使用领域事件拦截器（Using Max Domain Event Interceptor）
```csharp
public class LogDomainEventEventInterceptor : MaxDomainEventInterceptor
{
    public override Task BeforeExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        Console.WriteLine("LogDomainEventFilter.BeforeExecuteAsync");
        return base.BeforeExecuteAsync(context, cancellationToken);
    }

    public override Task AfterExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        Console.WriteLine("LogDomainEventFilter.AfterExecuteAsync");
        return base.AfterExecuteAsync(context, cancellationToken);
    }

    public override Task OnException(Exception ex, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context)
    {
        return base.OnException(ex, context);
    }
}
```
Ps：使用领域事件拦截器后，其会在对应的领域事件 Run 方法前与方法后 进行逻辑处理。
After using the domain event interceptor, it will perform logical processing before and after the corresponding domain event Run method.

### 1.3 领域事件拦截器中进行依赖注入（Dependency injection in domain event interceptors）
```csharp
public class LogDomainEventEventInterceptor : MaxDomainEventInterceptor
{
    private IService _service;

    //Constructor Inject
    //构造函数注入
    LogDomainEventEventInterceptor(IService service)
    {
        _service = service;
    }
    
    public override Task BeforeExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        Console.WriteLine("LogDomainEventFilter.BeforeExecuteAsync");
        return base.BeforeExecuteAsync(context, cancellationToken);
    }

    public override Task AfterExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        Console.WriteLine("LogDomainEventFilter.AfterExecuteAsync");
        return base.AfterExecuteAsync(context, cancellationToken);
    }

    public override Task OnException(Exception ex, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context)
    {
        return base.OnException(ex, context);
    }
}
```
