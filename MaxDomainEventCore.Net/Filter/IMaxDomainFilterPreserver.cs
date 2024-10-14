using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net.Filter;

public interface IMaxDomainFilterPreserver<T> where T : IMaxDomainFilterContext<IDomainEvent>
{
    List<IMaxDomainFilter<T>> Filters { get; }
    
    void AddMaxDomainFilter(IMaxDomainFilter<T> specification);

    Task<bool> ExecuteFilters();
}

public class MaxDomainFilterPreserver<T> : IMaxDomainFilterPreserver<T> where T : IMaxDomainFilterContext<IDomainEvent>
{
    private readonly List<IMaxDomainFilter<T>> _filters = new();

    public List<IMaxDomainFilter<T>> Filters => _filters;

    public void AddMaxDomainFilter(IMaxDomainFilter<T> specification)
    {
        _filters.Add(specification);
    }

    public async Task<bool> ExecuteFilters()
    {
        return await Task.FromResult(true);
    }
}