using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Interceptor.Interceptor;

public interface IMaxDomainEventInterceptor<in T>
    where T : IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    Task BeforeExecuteAsync(T context, CancellationToken cancellationToken);

    Task AfterExecuteAsync(T context, CancellationToken cancellationToken);
    
    Task OnException(Exception ex, T context);
}