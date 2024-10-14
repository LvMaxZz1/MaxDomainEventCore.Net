using Autofac;
using MaxDomainEventCore.Net.DomainEvents;
using MaxDomainEventCore.Net.Filter;
using MaxDomainEventCore.Net.Util.Max;
using MaxUtil.Net;

namespace MaxDomainEventCore.Net.Initiator;

public class DomainEventInitiator : IDomainEventInitiator
{
    private DomainEventRegister DomainEventRegister { get; } = new();

    private ILifetimeScope LifetimeScope { get; set; }

    private DomainHandler DomainHandler { get; set; }

    private IMaxDomainMessage DomainMessage { get; set; }


    public async Task PublishAsync<T>(T @event) where T : class, IDomainCommand<T>
    {
        DomainMessage.Message = @event;
        var handler = DomainEventRegister.GetAllHandlers()
            .FirstOrDefault(x => x.GetType() == typeof(Func<T, DomainEventInitiator, Task>));

        if (handler == null)
        {
            handler = MaxRegisterUtil.MakeNotResponseHandlerFunc(DomainHandler, @event.GetType(),
                typeof(DomainHandler)
                    .GetMethods().First(m =>
                        m.Name.Contains(nameof(DomainHandler.Handle)) && m.GetGenericArguments().Length == 1), this);
            DomainEventRegister.AddHandler(handler);
        }

        var resolveEvent = (T)LifetimeScope.Resolve(typeof(T));
        MaxDependencyInjectorUtil.InjectDependenciesFromSource(resolveEvent, @event);
        await ((Func<T, DomainEventInitiator, Task>)handler).Invoke(@event, this);
        //方法拦截器完全结束后释放消息
        DomainMessage.Dispose();
    }

    public async Task<TR> RequestAsync<T, TR>(T @event) where T : class, IDomainEvent where TR : class, IDomainResponse
    {
        DomainMessage.Message = @event;
        DomainMessage.Response = null;
        var handler = DomainEventRegister.GetAllHandlers()
            .FirstOrDefault(x => x.GetType() == typeof(Func<T, DomainEventInitiator, Task<TR>>));

        if (handler == null)
        {
            handler = MaxRegisterUtil.MakeNotResponseHandlerFunc(DomainHandler, @event.GetType(),
                typeof(DomainHandler)
                    .GetMethods().First(m =>
                        m.Name.Contains(nameof(DomainHandler.Handle)) && m.GetGenericArguments().Length == 2), this);
            DomainEventRegister.AddHandler(handler);
        }

        var resolveEvent = (T)LifetimeScope.Resolve(typeof(T));
        MaxDependencyInjectorUtil.InjectDependenciesFromSource(resolveEvent, @event);
        var response = await ((Func<T, DomainEventInitiator, Task<TR>>)handler).Invoke(@event, this);
        DomainMessage.Response = response;
        //方法拦截器完全结束后释放消息
        DomainMessage.Dispose();
        return response;
    }
}

public class DomainEventRegister
{
    private List<Delegate> Handlers { get; } = new();

    public void RegisterNotResponse<T>(Func<T, DomainEventInitiator, Task> handler) where T : class, IDomainEvent
    {
        Handlers.Add(handler);
    }

    public void RegisterHasResponse<T, TR>(Func<T, DomainEventInitiator, Task<TR>> handler) where T : class, IDomainEvent
    {
        Handlers.Add(handler);
    }

    internal void AddHandler(Delegate handler)
    {
        Handlers.Add(handler);
    }

    internal List<Delegate> GetAllHandlers()
    {
        return Handlers;
    }
}