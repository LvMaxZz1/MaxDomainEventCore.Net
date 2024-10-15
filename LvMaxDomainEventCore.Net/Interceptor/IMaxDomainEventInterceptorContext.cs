using LvMaxDomainEventCore.Net.DomainEvents;

namespace LvMaxDomainEventCore.Net.Interceptor;

public interface IMaxDomainEventInterceptorContext<T, TR>
    where T : class, IDomainEvent
    where TR : class, IDomainResponse
{
    T Message { get; set; }
    
    TR? Response { get; set; }
}

public class MaxDomainEventInterceptorContext<T, TR> : IMaxDomainEventInterceptorContext<T, TR>
    where T : class, IDomainEvent
    where TR : class, IDomainResponse
{
    public T Message { get; set; }
    
    public TR? Response { get; set; }
}