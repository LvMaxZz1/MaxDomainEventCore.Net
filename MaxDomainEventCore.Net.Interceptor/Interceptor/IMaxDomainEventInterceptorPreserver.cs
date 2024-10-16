using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Interceptor.Interceptor;

public interface IMaxDomainEventInterceptorPreserver<in T>
    where T : class, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    Task BeforeExecuteFilters(T maxDomainFilterContext, CancellationToken cancellationToken);

    Task AfterExecuteFilters(T maxDomainFilterContext,
        CancellationToken cancellationToken);

    Task OnException(Exception ex, T maxDomainFilterContext);
}