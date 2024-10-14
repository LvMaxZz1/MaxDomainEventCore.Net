using System.Reflection;
using Autofac.Core;

namespace MaxDomainEventCore.Net.AutofacDependency.DependencyProperty;

internal class MaxDependencyPropertySelector : IPropertySelector
{
    public bool InjectProperty(PropertyInfo propertyInfo, object instance)
    {
        // 带有 MaxDependencyPropertyAttribute 特性的属性 和 接口 都会进行属性注入
        return propertyInfo.CustomAttributes.Any(it => it.AttributeType == typeof(MaxDependencyPropertyAttribute)) || propertyInfo.PropertyType.IsInterface || propertyInfo.PropertyType.IsAbstract || propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.IsGenericType;
    }
}