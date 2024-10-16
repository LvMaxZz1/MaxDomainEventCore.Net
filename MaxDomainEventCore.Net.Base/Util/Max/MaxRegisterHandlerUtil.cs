using System.Reflection;
using MaxDomainEventCore.Net.Base.Initiator;
using MaxDomainEventCore.Net.Event.DomainEvents;

namespace MaxDomainEventCore.Net.Base.Util.Max;

public class MaxRegisterHandlerUtil
{
    /// <summary>
    /// Make有返回值的Handler方法
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventType"></param>
    /// <param name="handlerMethod"></param>
    /// <param name="returnType"></param>
    /// <param name="domainEventInitiator"></param>
    /// <returns></returns>
    public static Delegate MakeHasResponseHandlerFunc(DomainHandler handler, System.Type eventType, MethodInfo handlerMethod,
        System.Type returnType, IDomainEventInitiator domainEventInitiator)
    {
        var genericHandlerMethod = handlerMethod.MakeGenericMethod(eventType, returnType);
        var genericHandlerMethodFuncType =
            typeof(Func<,,,>).MakeGenericType(eventType, domainEventInitiator.GetType(), typeof(CancellationToken), 
                typeof(Task<>).MakeGenericType(returnType));
        var handlerFunc = Delegate.CreateDelegate(genericHandlerMethodFuncType, handler, genericHandlerMethod);
        return handlerFunc;
    }

    /// <summary>
    /// Make无返回值的Handler方法
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventType"></param>
    /// <param name="handlerMethod"></param>
    /// <param name="domainEventInitiator"></param>
    /// <returns></returns>
    public static Delegate MakeNotResponseHandlerFunc(DomainHandler handler, System.Type eventType, MethodInfo handlerMethod,
        IDomainEventInitiator domainEventInitiator)
    {
        var genericHandlerMethod = handlerMethod.MakeGenericMethod(eventType);
        var genericHandlerMethodActionType =
            typeof(Func<,,,>).MakeGenericType(eventType, domainEventInitiator.GetType(), typeof(CancellationToken),typeof(Task));
        var handlerAction = Delegate.CreateDelegate(genericHandlerMethodActionType, handler, genericHandlerMethod);
        return handlerAction;
    }
}