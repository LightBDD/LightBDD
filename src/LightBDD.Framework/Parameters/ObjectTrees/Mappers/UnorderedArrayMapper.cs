using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Mapper for collections implementing <seealso cref="IEnumerable"/> interface.
/// </summary>
public class UnorderedArrayMapper : ArrayMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly UnorderedArrayMapper Instance = new();
    private UnorderedArrayMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="IEnumerable"/>
    /// </summary>
    public override bool CanMap(object obj, ObjectTreeBuilderOptions options)
    {
        return obj is IEnumerable
               && IsUnorderedCollection(obj, options)
               && IsSortable(obj.GetType());
    }

    /// <summary>
    /// Maps provided instance to array and returns all it's items sorted in ascending order.
    /// </summary>
    public override ArrayMap MapArray(object o, ObjectTreeBuilderOptions options)
    {
        return new ArrayMap(((IEnumerable)o).Cast<object>().OrderBy(x => x));
    }

    private static bool IsUnorderedCollection(object obj, ObjectTreeBuilderOptions options)
    {
        var type = obj.GetType();
        return options.UnorderedCollectionTypes.Any(t => Reflector.IsImplementingType(type, t));
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