using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net.Filter;

public interface IMaxDomainFilter<in T> where T : IMaxDomainFilterContext<IDomainEvent>
{
    Task BeforeExecute(T context, CancellationToken cancellationToken);

    Task AfterExecute(T context, CancellationToken cancellationToken);
    
    Task OnException(Exception ex, T context);
}