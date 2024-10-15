using System.Reflection;
using Autofac;
using LvMaxDomainEventCore.Net.AutofacDependency.DependencyProperty;
using LvMaxDomainEventCore.Net.Dependency;
using LvMaxDomainEventCore.Net.DomainEvents;
using LvMaxDomainEventCore.Net.Initiator;
using LvMaxDomainEventCore.Net.Interceptor;

namespace LvMaxDomainEventCore.Net.Util.Max;

public abstract class MaxRegisterUtil
{
    /// <summary>
    /// 注册对应生命周期的依赖注入
    /// </summary>
    /// <param name="builder"></param>
    internal static void RegisterDependencies(ContainerBuilder builder)
    {
        foreach (var type in typeof(IMaxDependency).Assembly.GetTypes()
                     .Where(x => x.IsClass && typeof(IMaxDependency).IsAssignableFrom(x)))
        {
            if (typeof(IMaxScopeDependency).IsAssignableFrom(type))
            {
                if (type.GetInterfaces().Any(x=> x == typeof(IMaxNeedDependencyProperty)))
                {
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces().PropertiesAutowired(new MaxDependencyPropertySelector()).InstancePerLifetimeScope();
                }
                else
                {
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
                }
            }

            if (typeof(IMaxSingleDependency).IsAssignableFrom(type))
            {
                if (type.GetInterfaces().Any(x=> x == typeof(IMaxNeedDependencyProperty)))
                {
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces().PropertiesAutowired(new MaxDependencyPropertySelector()).SingleInstance();
                }
                else
                {
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces().SingleInstance();
                }
            }

            if (typeof(IMaxTransientDependency).IsAssignableFrom(type))
            {
                if (type.GetInterfaces().Any(x=> x == typeof(IMaxNeedDependencyProperty)))
                {
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces().PropertiesAutowired(new MaxDependencyPropertySelector());
                }
                else
                {
                    builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
                }
            }
        }
    }

    /// <summary>
    /// 注册Event
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="entityTypes"></param>
    internal static void RegisterEvents(ContainerBuilder builder, List<System.Type> entityTypes)
    {
        foreach (var eventType in entityTypes)
        {
            builder.RegisterType(eventType).AsSelf().PropertiesAutowired(new MaxDependencyPropertySelector())
                .InstancePerLifetimeScope();
        }
    }

    /// <summary>
    /// 注册Handler
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventTypes"></param>
    /// <param name="domainEventRegister"></param>
    /// <param name="domainEventInitiator"></param>
    /// <exception cref="Exception"></exception>
    internal static void RegisterHandlers(DomainHandler handler, List<System.Type> eventTypes,
        DomainEventRegister domainEventRegister, IDomainEventInitiator domainEventInitiator)
    {
        eventTypes.ForEach(eventType =>
        {
            var handleMethods = typeof(DomainHandler).GetMethods()
                .Where(m => m.Name.Contains(nameof(DomainHandler.Handle)));

            foreach (var handlerMethod in handleMethods)
            {
                //根据实现的接口调用不同的注册方法
                var interfaceType = eventType.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainCommand<>));

                if (eventType.GetInterfaces().Any(x =>
                        x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDomainCommand<>)) && eventType
                        .GetInterfaces().Any(x =>
                            x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDomainRequest<,>)))
                {
                    throw new Exception(
                        $"Event : {eventType.Name} cannot duplicate registration requests and commands");
                }

                if (interfaceType != null)
                {
                    RegisterNotResponseHandler(handler, eventType, handlerMethod, domainEventRegister, domainEventInitiator);
                }
                else
                {
                    var genericInterfaceType = eventType.GetInterfaces()
                        .FirstOrDefault(i =>
                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainRequest<,>));

                    if (genericInterfaceType != null)
                    {
                        //获得返回值的实际类型
                        var genericArgs = genericInterfaceType.GetGenericArguments();
                        var returnType = genericArgs[1];
                        RegisterHasResponseHandler(handler, eventType, handlerMethod, returnType, domainEventRegister,
                            domainEventInitiator);
                    }
                }
            }
        });
    }

    /// <summary>
    /// 注册无返回值的处理器
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventType"></param>
    /// <param name="handlerMethod"></param>
    /// <param name="domainEventRegister"></param>
    /// <param name="domainEventInitiator"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private static void RegisterNotResponseHandler(
        DomainHandler handler, System.Type eventType, MethodInfo handlerMethod, DomainEventRegister domainEventRegister,
        IDomainEventInitiator domainEventInitiator)
    {
        var registerMethod = typeof(DomainEventRegister).GetMethod(nameof(DomainEventRegister.RegisterNotResponse));

        if (registerMethod == null)
            throw new InvalidOperationException(
                $"Could not find the Register method for type {nameof(DomainEventRegister)}.");

        // 确保方法参数和泛型参数数量一致
        if (handlerMethod.GetGenericArguments().Length != registerMethod.GetGenericArguments().Length) return;

        // 确保 Register 方法是泛型的
        if (!registerMethod.IsGenericMethodDefinition)
            throw new InvalidOperationException("Register method is not a generic method definition.");

        //创建装备了实际参数的Handle方法委托
        var handlerAction = MakeNotResponseHandlerFunc(handler, eventType, handlerMethod, domainEventInitiator);

        //创建注册Register方法装备参数并执行
        var genericRegisterMethod = registerMethod.MakeGenericMethod(eventType);
        genericRegisterMethod.Invoke(domainEventRegister, [handlerAction]);
    }

    /// <summary>
    /// 注册有返回值的处理器
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventType"></param>
    /// <param name="handlerMethod"></param>
    /// <param name="returnType"></param>
    /// <param name="domainEventRegister"></param>
    /// <param name="domainEventInitiator"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private static void RegisterHasResponseHandler(
        DomainHandler handler, System.Type eventType, MethodInfo handlerMethod, System.Type returnType,
        DomainEventRegister domainEventRegister, IDomainEventInitiator domainEventInitiator)
    {
        var registerMethod = typeof(DomainEventRegister).GetMethod(nameof(DomainEventRegister.RegisterHasResponse));

        if (registerMethod == null)
            throw new InvalidOperationException($"Could not find the Register method for type {eventType.FullName}.");

        if (handlerMethod.GetGenericArguments().Length != registerMethod.GetGenericArguments().Length) return;

        // 确保 Register 方法是泛型的
        if (!registerMethod.IsGenericMethodDefinition)
            throw new InvalidOperationException("Register method is not a generic method definition.");


        //创建装备了实际参数的Handle方法委托
        var handlerFunc = MakeHasResponseHandlerFunc(handler, eventType, handlerMethod, returnType, domainEventInitiator);

        //创建注册Register方法装备参数并执行
        var genericRegisterMethod = registerMethod.MakeGenericMethod(eventType, returnType);
        genericRegisterMethod.Invoke(domainEventRegister, [handlerFunc]);
    }

    /// <summary>
    /// Make有返回值的Handler方法
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="eventType"></param>
    /// <param name="handlerMethod"></param>
    /// <param name="returnType"></param>
    /// <param name="domainEventInitiator"></param>
    /// <returns></returns>
    private static Delegate MakeHasResponseHandlerFunc(DomainHandler handler, System.Type eventType, MethodInfo handlerMethod,
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
    internal static Delegate MakeNotResponseHandlerFunc(DomainHandler handler, System.Type eventType, MethodInfo handlerMethod,
        IDomainEventInitiator domainEventInitiator)
    {
        var genericHandlerMethod = handlerMethod.MakeGenericMethod(eventType);
        var genericHandlerMethodActionType =
            typeof(Func<,,,>).MakeGenericType(eventType, domainEventInitiator.GetType(), typeof(CancellationToken),typeof(Task));
        var handlerAction = Delegate.CreateDelegate(genericHandlerMethodActionType, handler, genericHandlerMethod);
        return handlerAction;
    }
    
    /// <summary>
    /// 注册领域事件拦截器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="filterTypes"></param>
    /// <param name="eventInterceptorPreserver"></param>
    internal static void RegisterDomainEventInterceptor(ContainerBuilder builder, List<System.Type> filterTypes,
        MaxDomainEventInterceptorPreserver<IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>> eventInterceptorPreserver)
    {
        if (filterTypes.Any())
        {
            filterTypes.ForEach(x =>
            {
                var instance = Activator.CreateInstance(x);
                if (instance is MaxDomainEventInterceptor filter)
                {
                    eventInterceptorPreserver.AddMaxDomainFilter(filter);
                }
            });
        }

        builder.RegisterInstance(eventInterceptorPreserver).AsSelf().AsImplementedInterfaces().SingleInstance();
    }
}