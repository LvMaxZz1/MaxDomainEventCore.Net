using System.Reflection;

namespace MaxDomainEventCore.Net.Base.Util.Max;

public abstract class MaxDependencyInjectorUtil
{
    /// <summary>
    /// Obtain dependency relationships from the source and inject them into the destination
    /// If the class structure is inconsistent, it will fail
    /// 从源获取依赖关系并将其注入目标,如果类结构不一致则会失败
    /// </summary>
    /// <param name="source">From whom do we obtain dependencies?</param>
    /// <param name="destination">To whom will we pass on our dependencies</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    public static void InjectDependenciesFromSource<T, TR>(T source, TR destination)
        where T : class
        where TR : class
    {
        var sourceType = typeof(T);
        var destinationType = typeof(TR);
        if (destinationType.Name != sourceType.Name)
        {
            return;
        }
        // 遍历所有属性
        var desPropertyInfos = destinationType.GetTypeInfo().DeclaredProperties
            .Where(x => 
                (x.PropertyType.IsInterface || x.PropertyType.IsClass || x.PropertyType.IsGenericType) && 
                !x.PropertyType.IsPrimitive && 
                x.PropertyType != typeof(string) && 
                x.PropertyType != typeof(object) &&
                !(x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)));
        foreach (var desPropertyInfo in desPropertyInfos)
        {
            if (desPropertyInfo.CanWrite)
            {
                var value = desPropertyInfo.GetValue(source);
                if (value != null)
                {
                    desPropertyInfo.SetValue(destination, value);
                }
            }
        }
    }
}