using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace MaxDomainEventCore.Net.Base.Util.Type;

public abstract class TypeUtil
{
    /// <summary>
    /// Obtain all implementation types
    /// 获得所有实现类型
    /// </summary>
    /// <param name="implementedBy">Whose implementation type do we need to obtain?</param>
    /// <returns></returns>
    public static List<System.Type> ObtainImplementer(System.Type implementedBy)
    {
        List<System.Type> entityTypes = [];
        var libs = DependencyContext.Default.CompileLibraries
            .Where(x => !x.Serviceable && x.Type != "package" && x.Type == "project");
        foreach (var lib in libs)
        {
            var currentTypes = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name))
                .GetTypes()
                .Where(x =>
                    x.GetTypeInfo().BaseType != null
                    && x is { IsAbstract: false, IsClass: true, IsGenericType: false }
                    && implementedBy.IsAssignableFrom(x)
                )
                .ToList();
            if (currentTypes.Any()) entityTypes.AddRange(currentTypes);
        }

        return entityTypes;
    }

    /// <summary>
    /// Obtain all implementation types
    /// 获得所有实现类型
    /// </summary>
    /// <typeparam name="T">Whose implementation type do we need to obtain?</typeparam>
    /// <returns></returns>
    public static List<System.Type> ObtainImplementer<T>()
    {
        var implementedBy = typeof(T);
        return ObtainImplementer(implementedBy);
    }


    public static List<System.Type> ObtainGenericImplementer(System.Type implementedBy)
    {
        List<System.Type> entityTypes = [];
        var libs = DependencyContext.Default.CompileLibraries
            .Where(x => !x.Serviceable && x.Type != "package" && x.Type == "project");
        foreach (var lib in libs)
        {
            var currentTypes = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name)).GetTypes()
                .Where(x => x is { IsAbstract: false, IsClass: true, IsGenericType: true } && x.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == implementedBy)).ToList();
            if (currentTypes.Any()) entityTypes.AddRange(currentTypes);
        }

        return entityTypes;
    }

    public static List<System.Type> ObtainGenericImplementer<T>()
    {
        var implementedBy = typeof(T);
        return ObtainGenericImplementer(implementedBy);
    }
}