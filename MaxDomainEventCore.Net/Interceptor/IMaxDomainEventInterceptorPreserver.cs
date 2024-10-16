using Autofac;
using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net.Interceptor;

internal interface IMaxDomainEventInterceptorPreserver<in T>
    where T : class, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    Task BeforeExecuteFilters(T maxDomainFilterContext, CancellationToken cancellationToken);

    Task AfterExecuteFilters(T maxDomainFilterContext,
        CancellationToken cancellationToken);

    Task OnException(Exception ex, T maxDomainFilterContext);
}

internal class MaxDomainEventInterceptorPreserver<T> : IMaxDomainEventInterceptorPreserver<T>
    where T : class, IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>
{
    private readonly List<IMaxDomainEventInterceptor<T>> _filters = new List<IMaxDomainEventInterceptor<T>>();
    
    private  readonly List<Type> _interceptorTypes = new List<Type>();
    
    private ILifetimeScope LifetimeScope { get; set; }
    
    private  int _index = 0;

    private bool IsExecuted => _index < _filters.Count;

    public List<IMaxDomainEventInterceptor<T>> Filters => _filters;

    internal void AddMaxDomainFilter(IMaxDomainEventInterceptor<T> specification)
    {
        _filters.Add(specification);
    }

    internal void AddMaxDomainFilterType(Type interceptorType)
    {
        if (typeof(MaxDomainEventInterceptor).IsAssignableFrom(interceptorType))
        {
            _interceptorTypes.Add(interceptorType);
        }
    }
    
    internal void AddMaxDomainFilterTypes(Type[] interceptorType)
    {
        foreach (var type in interceptorType)
        {
            AddMaxDomainFilterType(type);
        }
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
    
    internal void InitializeInterceptor()
    {
        _interceptorTypes.ForEach(x =>
        {
            var interceptor = LifetimeScope.Resolve(x);
            if (interceptor is MaxDomainEventInterceptor maxInterceptor)
            {
                AddMaxDomainFilter(maxInterceptor);
            }
        });
    }
}