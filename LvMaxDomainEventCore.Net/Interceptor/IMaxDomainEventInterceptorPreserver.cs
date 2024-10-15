using LvMaxDomainEventCore.Net.DomainEvents;

namespace LvMaxDomainEventCore.Net.Interceptor;

internal interface IMaxDomainEventInterceptorPreserver<T>
    where T : class, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    List<IMaxDomainEventInterceptor<T>> Filters { get; }
    
    void AddMaxDomainFilter(IMaxDomainEventInterceptor<T> specification);

    Task BeforeExecuteFilters(T maxDomainFilterContext, CancellationToken cancellationToken);

    Task AfterExecuteFilters(T maxDomainFilterContext,
        CancellationToken cancellationToken);

    Task OnException(Exception ex, T maxDomainFilterContext);
}

internal class MaxDomainEventInterceptorPreserver<T> : IMaxDomainEventInterceptorPreserver<T>
    where T : class, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    private readonly List<IMaxDomainEventInterceptor<T>> _filters = new List<IMaxDomainEventInterceptor<T>>();
    
    private  int _index = 0;

    private bool IsExecuted => _index < _filters.Count;

    public List<IMaxDomainEventInterceptor<T>> Filters => _filters;

    public void AddMaxDomainFilter(IMaxDomainEventInterceptor<T> specification)
    {
        _filters.Add(specification);
    }

    public async Task BeforeExecuteFilters(T maxDomainFilterContext, CancellationToken cancellationToken)
    {
        await this.BeforeExecuteNextAsync(maxDomainFilterContext, cancellationToken);
    }
    
    public async Task AfterExecuteFilters(T maxDomainFilterContext, CancellationToken cancellationToken)
    {
        await this.AfterExecuteAsyncNextAsync(maxDomainFilterContext, cancellationToken);
    }

    public async Task OnException(Exception ex, T maxDomainFilterContext)
    {
        await this.ExecuteAsync(ex, maxDomainFilterContext);
    }

    private async Task ExecuteAsync(Exception exception, T maxDomainFilterContext)
    {
        if (IsExecuted)
        {
            await _filters[_index].OnException(exception, maxDomainFilterContext);
            _index++;
            await this.OnException(exception, maxDomainFilterContext);
        }
        _index = 0;
    }

    private async Task BeforeExecuteNextAsync(T maxDomainFilterContext, CancellationToken cancellationToken)
    {
        if (IsExecuted)
        {
            await _filters[_index].BeforeExecuteAsync(maxDomainFilterContext, cancellationToken);
            _index++;
            await this.BeforeExecuteNextAsync(maxDomainFilterContext, cancellationToken);
        }
        _index = 0;
    }
    
    private async Task AfterExecuteAsyncNextAsync(T maxDomainFilterContext, CancellationToken cancellationToken)
    {
        if (IsExecuted)
        {
            await _filters[_index].AfterExecuteAsync(maxDomainFilterContext, cancellationToken);
            _index++;
            await this.AfterExecuteAsyncNextAsync(maxDomainFilterContext, cancellationToken);
        }
        _index = 0;
    }
}