using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Base.Initiator;

public class DomainEventRegister
{
    private List<Delegate> Handlers { get; } = new();

    public void RegisterNotResponse<T>(Func<T, DomainEventInitiator, CancellationToken ,Task> handler) where T : class, IDomainEvent
    {
        Handlers.Add(handler);
    }

    public void RegisterHasResponse<T, TR>(Func<T, DomainEventInitiator, CancellationToken, Task<TR>> handler)
        where T : class, IDomainEvent
    {
        Handlers.Add(handler);
    }

    internal void AddHandler(Delegate handler)
    {
        Handlers.Add(handler);
    }

    internal List<Delegate> GetAllHandlers()
    {
        return Handlers;
    }
}