using System.Runtime.ExceptionServices;
using LvMaxDomainEventCore.Net.DomainEvents;

namespace LvMaxDomainEventCore.Net.Interceptor;

public interface IMaxDomainEventInterceptor<in T>
    where T : IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    Task BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    Task AfterExecuteAsync(T context, CancellationToken cancellationToken);
    
    Task OnException(Exception ex, T context);
}

public abstract class MaxDomainEventInterceptor : IMaxDomainEventInterceptor<IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>>
{
    public virtual async Task BeforeExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    { 
        await Task.CompletedTask;
    }

    public virtual async Task AfterExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public virtual async Task OnException(Exception ex, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context)
    {
        ExceptionDispatchInfo.Capture(ex).Throw();
        await Task.CompletedTask;
    }
}