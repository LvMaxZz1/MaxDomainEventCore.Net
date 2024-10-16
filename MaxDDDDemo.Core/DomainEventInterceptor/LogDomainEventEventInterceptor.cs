using MaxDomainEventCore.Net.DomainEvents;
using MaxDomainEventCore.Net.Interceptor;

namespace MaxDDDDemo.Core.DomainEventInterceptor;

public class LogDomainEventEventInterceptor : MaxDomainEventInterceptor
{
    public override Task BeforeExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        return base.BeforeExecuteAsync(context, cancellationToken);
    }

    public override Task AfterExecuteAsync(IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context, CancellationToken cancellationToken)
    {
        return base.AfterExecuteAsync(context, cancellationToken);
    }

    public override Task OnException(Exception ex, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse> context)
    {
        return base.OnException(ex, context);
    }
}