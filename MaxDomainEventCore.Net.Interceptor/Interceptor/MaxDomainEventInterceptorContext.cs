using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Interceptor.Interceptor;

public class MaxDomainEventInterceptorContext<T, TR> : IMaxDomainEventInterceptorContext<T, TR>
    where T : class, IDomainEvent
    where TR : class, IDomainResponse
{
    public T Message { get; set; }
    
    public TR? Response { get; set; }
}