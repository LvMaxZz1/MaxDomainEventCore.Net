using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Interceptor.Interceptor;

public interface IMaxDomainEventInterceptorContext<T, TR>
    where T : class, IDomainEvent
    where TR : class, IDomainResponse
{
    T Message { get; set; }
    
    TR? Response { get; set; }
}