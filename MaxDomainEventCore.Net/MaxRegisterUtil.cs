using System.Reflection;
using Autofac;
using MaxDomainEventCore.Net.AutofacDependency.DependencyProperty;
using MaxDomainEventCore.Net.Dependency;
using MaxDomainEventCore.Net.DomainEvents;

namespace MaxDomainEventCore.Net;

public abstract class MaxRegisterUtil
{
    public static void RegisterDependencies(ContainerBuilder builder)
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

    public static void RegisterEvents(ContainerBuilder builder, List<Type> entityTypes)
    {
        foreach (var eventType in entityTypes)
        {
            builder.RegisterType(eventType).AsSelf().PropertiesAutowired(new MaxDependencyPropertySelector())
                .InstancePerLifetimeScope();
        }
    }

    public static void RegisterHandlers(DomainHandler handler, List<Type> eventTypes,
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

    private static void RegisterNotResponseHandler(
        DomainHandler handler, Type eventType, MethodInfo handlerMethod, DomainEventRegister domainEventRegister,
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

    private static void RegisterHasResponseHandler(
        DomainHandler handler, Type eventType, MethodInfo handlerMethod, Type returnType,
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

    private static Delegate MakeHasResponseHandlerFunc(DomainHandler handler, Type eventType, MethodInfo handlerMethod,
        Type returnType, IDomainEventInitiator domainEventInitiator)
    {
        var genericHandlerMethod = handlerMethod.MakeGenericMethod(eventType, returnType);
        var genericHandlerMethodFuncType =
            typeof(Func<,,>).MakeGenericType(eventType, domainEventInitiator.GetType(),
                typeof(Task<>).MakeGenericType(returnType));
        var handlerFunc = Delegate.CreateDelegate(genericHandlerMethodFuncType, handler, genericHandlerMethod);
        return handlerFunc;
    }

    public static Delegate MakeNotResponseHandlerFunc(DomainHandler handler, Type eventType, MethodInfo handlerMethod,
        IDomainEventInitiator domainEventInitiator)
    {
        var genericHandlerMethod = handlerMethod.MakeGenericMethod(eventType);
        var genericHandlerMethodActionType =
            typeof(Func<,,>).MakeGenericType(eventType, domainEventInitiator.GetType(), typeof(Task));
        var handlerAction = Delegate.CreateDelegate(genericHandlerMethodActionType, handler, genericHandlerMethod);
        return handlerAction;
    }
}