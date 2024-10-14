using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net.Filter;

public interface IMaxDomainFilterContext<out  T> where T : class, IDomainEvent
{
    T Message { get; }
    
    object? Response { get; }
}

public class MaxDomainFilterContext<T> : IMaxDomainFilterContext<T> where T : class, IDomainEvent
{
    public T Message { get; }
    
    public object? Response { get; set; }
}