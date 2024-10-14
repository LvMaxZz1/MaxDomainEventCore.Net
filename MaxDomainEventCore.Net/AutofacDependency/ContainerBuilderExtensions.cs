using System.Linq;
using System.Reflection;
using Autofac;
using MaxDomainEventCore.Net.AutofacDependency.DependencyProperty;
using MaxDomainEventCore.Net.DomainEvents;
using MaxUtil.Net.Type;

namespace MaxDomainEventCore.Net.AutofacDependency;

public static class ContainerBuilderExtensions
{
    public static void RegisterMaxEvent(this ContainerBuilder builder)
    {
        var handler = new DomainHandler();
        var eventBus = new DomainEventInitiator();
        var newRegister = new DomainEventRegister();
        // 创建一个新的 DomainEventRegister 实例用于存储事件处理器并赋值给DomainEventBus的对应字段
        typeof(DomainEventInitiator).GetTypeInfo().DeclaredFields
            .First(x => x.Name.Contains(nameof(IDomainEventRegisterName.DomainEventRegister)))
            .SetValue(eventBus, newRegister);

        //反射找到所有实现了IDomainEvent的类,并且注册服务,同时注册对应handle方法
        var entityTypes = TypeUtil.ObtainImplementer<IDomainEvent>();
        MaxRegisterUtil.RegisterEvents(builder, entityTypes);
        MaxRegisterUtil.RegisterHandlers(handler, entityTypes, newRegister, eventBus);
        MaxRegisterUtil.RegisterDependencies(builder);

        builder.RegisterInstance(eventBus).AsSelf().AsImplementedInterfaces()
            .PropertiesAutowired(new MaxDependencyPropertySelector()).SingleInstance();
    }
}



internal interface IDomainEventRegisterName
{
    string DomainEventRegister { get; }
}