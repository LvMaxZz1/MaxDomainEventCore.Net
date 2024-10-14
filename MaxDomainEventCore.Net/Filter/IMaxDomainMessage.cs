using MaxDomainEventCore.Net.Dependency;

namespace MaxDomainEventCore.Net.Filter;

internal interface IMaxDomainMessage : IDisposable, IMaxTransientDependency
{
    object? Message { get; set; }
    object? Response { get; set; }
}

public class MaxDomainMessage : IMaxDomainMessage
{
    public object? Message { get; set; }

    public object? Response { get; set; }

    public void Dispose()
    {
        Message = null;
        Response = null;
    }
}