using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Mapper for collections implementing <seealso cref="IEnumerable"/> interface.
/// </summary>
public class EnumerableMapper : ArrayMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly EnumerableMapper Instance = new();
    private EnumerableMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="IEnumerable"/>
    /// </summary>
    public override bool CanMap(object obj) => obj is IEnumerable;

    /// <summary>
    /// Enumerates collection items.<br/>
    /// If collection is un-ordered and items are sortable, the returned items will be sorted.<br/>
    /// In all other cases, items will be returned in enumeration order.
    /// </summary>
    public override ArrayMap MapArray(object o)
    {
        var type = o.GetType();
        if (IsOrdered(type) || !IsSortable(type))
            return new ArrayMap(((IEnumerable)o).Cast<object>());
        return new ArrayMap(((IEnumerable)o).Cast<object>().OrderBy(x => x));
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