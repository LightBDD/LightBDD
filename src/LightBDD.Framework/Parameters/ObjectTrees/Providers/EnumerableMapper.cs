using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class EnumerableMapper : ArrayMapper
{
    public static readonly EnumerableMapper Instance = new();
    private EnumerableMapper() { }

    public override bool CanMap(object obj) => obj is IEnumerable;
    public override IEnumerable<object> GetItems(object o)
    {
        var type = o.GetType();
        if (IsOrdered(type) || !IsSortable(type))
            return ((IEnumerable)o).Cast<object>();
        return ((IEnumerable)o).Cast<object>().OrderBy(x => x);
    }

    private static bool IsOrdered(Type t)
    {
        return GetGenericInterfaceDefinitions(t)
            .Any(genericInterface => genericInterface == typeof(IList<>)
                                     || genericInterface == typeof(IReadOnlyList<>)
                                     || genericInterface == typeof(IOrderedEnumerable<>));
    }

    private static bool IsSortable(Type collection)
    {
        var enumerableType = collection.GetInterfaces().Where(i => i.IsGenericType)
            .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableType == null)
            return false;

        var itemType = enumerableType.GetGenericArguments().First();

        return typeof(IComparable).IsAssignableFrom(itemType) || GetGenericInterfaceDefinitions(itemType).Any(i => i == typeof(IComparable<>));
    }

    private static IEnumerable<Type> GetGenericInterfaceDefinitions(Type t)
    {
        return t.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
    }
}