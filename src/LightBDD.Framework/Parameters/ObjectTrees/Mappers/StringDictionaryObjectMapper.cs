#nullable enable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Mapper for <seealso cref="IDictionary{TKey,TValue}"/> and <seealso cref="IReadOnlyDictionary{TKey,TValue}"/> of <seealso cref="string"/> key, that represents it as object node.
/// </summary>
public class StringDictionaryObjectMapper : ObjectMapper
{
    private readonly ConcurrentDictionary<Type, Func<object, IEnumerable<ObjectProperty>>?> _map = new();
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly StringDictionaryObjectMapper Instance = new();
    private StringDictionaryObjectMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="IDictionary{TKey,TValue}"/> and <seealso cref="IReadOnlyDictionary{TKey,TValue}"/> where it's key parameter is <seealso cref="string"/>
    /// </summary>
    public override bool CanMap(object obj, ObjectTreeBuilderOptions options) => obj is IEnumerable && _map.GetOrAdd(obj.GetType(), GetPropertyEnumerator) != null;

    private static Func<object, IEnumerable<ObjectProperty>>? GetPropertyEnumerator(Type type)
    {
        return (type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .Where(IsDictionaryInterface)
            .Select(i => MakeGenericDelegateFor(i.GetGenericArguments()[1])))
            .FirstOrDefault();
    }

    /// <summary>
    /// Maps provided dictionary as object node.
    /// </summary>
    public override ObjectMap MapObject(object o, ObjectTreeBuilderOptions options)
    {
        var getPropertiesFn = _map[o.GetType()] ?? throw new InvalidOperationException($"{o.GetType()} does not represent string dictionary");
        return new ObjectMap(getPropertiesFn.Invoke(o));
    }

    private static IEnumerable<ObjectProperty> EnumerateDictionary<TValue>(object o)
    {
        return ((IEnumerable<KeyValuePair<string, TValue>>)o).Select(p => new ObjectProperty(p.Key, p.Value));
    }

    private static Func<object, IEnumerable<ObjectProperty>>? MakeGenericDelegateFor(Type valueType)
    {
        var func = EnumerateDictionary<object>;
        var targetFunc = func.Method.GetGenericMethodDefinition().MakeGenericMethod(valueType);
        return (Func<object, IEnumerable<ObjectProperty>>?)Delegate.CreateDelegate(typeof(Func<object, IEnumerable<ObjectProperty>>), targetFunc);
    }

    private static bool IsDictionaryInterface(Type genericType)
    {
        var genericTypeDefinition = genericType.GetGenericTypeDefinition();
        return (genericTypeDefinition == typeof(IDictionary<,>) ||
                genericTypeDefinition == typeof(IReadOnlyDictionary<,>)) &&
               genericType.GetGenericArguments()[0] == typeof(string);
    }
}