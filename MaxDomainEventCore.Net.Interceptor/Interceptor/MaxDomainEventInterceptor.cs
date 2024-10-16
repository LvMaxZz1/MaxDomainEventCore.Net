using System.Runtime.ExceptionServices;
using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Interceptor.Interceptor;

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