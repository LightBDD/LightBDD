#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;
/// <summary>
/// Object mapper for POCO types.
/// </summary>
public class PocoMapper : ObjectMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly PocoMapper Instance = new();

    private readonly ConcurrentDictionary<Type, IReadOnlyList<KeyValuePair<string, Func<object, object?>>>> _typeMap = new();
    private static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public;

    private PocoMapper() { }

    /// <summary>
    /// Always returns <c>true</c>
    /// </summary>
    public override bool CanMap(object obj) => true;

    /// <summary>
    /// Maps POCO to object where it returns public instance fields and properties of the <paramref name="o"/>.
    /// </summary>
    public override ObjectMap MapObject(object o)
    {
        return new ObjectMap(GetProperties(o));
    }

    private IEnumerable<ObjectProperty> GetProperties(object o)
    {
        var type = o.GetType();
        var map = _typeMap.GetOrAdd(type, MapType);
        return map.Select(p => new ObjectProperty(p.Key, GetValue(o, p)));
    }

    private static object? GetValue(object o, KeyValuePair<string, Func<object, object?>> p)
    {
        try
        {
            return p.Value(o);
        }
        catch (TargetInvocationException ex)
        {
            return new ExceptionCapture(ex.InnerException ?? ex);
        }
    }

    private IReadOnlyList<KeyValuePair<string, Func<object, object?>>> MapType(Type type)
    {
        var map = new List<KeyValuePair<string, Func<object, object?>>>();

        var fields = type.GetFields(BindingFlags)
            .GroupBy(f => f.Name)
            .Select(GetNewest);

        foreach (var field in fields)
            map.Add(new KeyValuePair<string, Func<object, object?>>(field.Name, field.GetValue));

        var props = type.GetProperties(BindingFlags)
            .Where(x => x.CanRead && !x.GetIndexParameters().Any())
            .GroupBy(f => f.Name)
            .Select(GetNewest);

        foreach (var prop in props)
            map.Add(new KeyValuePair<string, Func<object, object?>>(prop.Name, prop.GetValue));

        return map;
    }

    private TMemberInfo GetNewest<TMemberInfo>(IEnumerable<TMemberInfo> members) where TMemberInfo : MemberInfo
    {
        TMemberInfo? result = null;
        foreach (var m in members)
        {
            if (result == null || result.DeclaringType!.IsAssignableFrom(m.DeclaringType))
                result = m;
        }

        return result!;
    }
}