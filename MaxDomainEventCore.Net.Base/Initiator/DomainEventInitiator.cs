using Autofac;
using MaxDomainEventCore.Net.Base.Util.Max;
using MaxDomainEventCore.Net.Event.DomainEvents;
using MaxDomainEventCore.Net.Interceptor.Interceptor;

namespace MaxDomainEventCore.Net.Base.Initiator;

public class DomainEventInitiator : IDomainEventInitiator
{
    private DomainEventRegister DomainEventRegister { get; } = new();

    private ILifetimeScope LifetimeScope { get; set; }

    private DomainHandler DomainHandler { get; set; }

    private IMaxDomainEventInterceptorPreserver<IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>> EventInterceptorPreserver
    {
        get;
        set;
    }

    private IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> DomainEventInterceptorContext { get; set; } =
        new MaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>();


    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : class, IDomainCommand<T>
    {
        try
        {
            DomainEventInterceptorContext.Message = @event;
            var handler = DomainEventRegister.GetAllHandlers()
                .FirstOrDefault(x => x.GetType() == typeof(Func<T, DomainEventInitiator, CancellationToken, Task>));

            handler = RegisterNoResponseHandlerFuncIfNeeded(@event, handler);

            var resolveEvent = (T)LifetimeScope.Resolve(typeof(T));
            MaxDependencyInjectorUtil.InjectDependenciesFromSource(resolveEvent, @event);
            
            await EventInterceptorPreserver.BeforeExecuteFilters(DomainEventInterceptorContext, cancellationToken);
            await ((Func<T, DomainEventInitiator, CancellationToken, Task>)handler).Invoke(@event, this, cancellationToken);
            DomainEventInterceptorContext.Message = @event;
            DomainEventInterceptorContext.Response = null;
            await EventInterceptorPreserver.AfterExecuteFilters(DomainEventInterceptorContext, cancellationToken);
        }
        catch (Exception ex)
        {
            await EventInterceptorPreserver.OnException(ex, DomainEventInterceptorContext);
        }
    }

    public async Task<TR> RequestAsync<T, TR>(T @event, CancellationToken cancellationToken = default)
        where T : class, IDomainEvent where TR : class, IDomainResponse
    {
        try
        {
            DomainEventInterceptorContext.Message = @event;
            TR response;
            var handler = DomainEventRegister.GetAllHandlers()
                .FirstOrDefault(x => x.GetType() == typeof(Func<T, DomainEventInitiator, CancellationToken, Task<TR>>));

            handler = RegisterHasResponseHandlerFuncIfNeeded<T>(@event, handler);

            var resolveEvent = (T)LifetimeScope.Resolve(typeof(T));
            MaxDependencyInjectorUtil.InjectDependenciesFromSource(resolveEvent, @event);

            await EventInterceptorPreserver.BeforeExecuteFilters(DomainEventInterceptorContext, cancellationToken);
            response = await ((Func<T, DomainEventInitiator, CancellationToken, Task<TR>>)handler).Invoke(@event, this, cancellationToken);
            DomainEventInterceptorContext.Response = response;
            DomainEventInterceptorContext.Message = @event;
            await EventInterceptorPreserver.AfterExecuteFilters(DomainEventInterceptorContext, cancellationToken);
            return response;
        }
        catch (Exception ex)
        { 
            await EventInterceptorPreserver.OnException(ex, DomainEventInterceptorContext);
            return (TR)default;
        }
    }

    public async Task<TR> SendAsync<T, TR>(T @event, CancellationToken cancellationToken = default)
        where T : class, IDomainEvent where TR : class, IDomainResponse
    {
        var response = await RequestAsync<T, TR>(@event, cancellationToken);
        return response;
    }

    private Delegate RegisterNoResponseHandlerFuncIfNeeded<T>(T @event, Delegate? handler)
        where T : class, IDomainCommand<T>
    {
        if (handler == null)
        {
            handler = MaxRegisterHandlerUtil.MakeNotResponseHandlerFunc(DomainHandler, @event.GetType(),
                typeof(DomainHandler)
                    .GetMethods().First(m =>
                        m.Name.Contains(nameof(DomainHandler.Handle)) && m.GetGenericArguments().Length == 1),
                this);
            DomainEventRegister.AddHandler(handler);
        }

        return handler;
    }

    private Delegate RegisterHasResponseHandlerFuncIfNeeded<T>(T @event, Delegate? handler)
        where T : class, IDomainEvent
    {
        if (handler == null)
        {
            handler = MaxRegisterHandlerUtil.MakeNotResponseHandlerFunc(DomainHandler, @event.GetType(),
                typeof(DomainHandler)
                    .GetMethods().First(m =>
                        m.Name.Contains(nameof(DomainHandler.Handle)) && m.GetGenericArguments().Length == 2),
                this);
            DomainEventRegister.AddHandler(handler);
        }

        return handler;
    }
}