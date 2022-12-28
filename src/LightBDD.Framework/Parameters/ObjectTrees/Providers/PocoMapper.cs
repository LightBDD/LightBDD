#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class PocoMapper : ObjectMapper
{
    public static readonly PocoMapper Instance = new();
    private readonly ConcurrentDictionary<Type, IReadOnlyList<KeyValuePair<string, Func<object, object>>>> _typeMap = new();
    private static readonly BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public;

    private PocoMapper() { }

    public override bool CanMap(object obj) => true;

    public override IEnumerable<KeyValuePair<string, object?>> GetProperties(object o)
    {
        var type = o.GetType();
        var map = _typeMap.GetOrAdd(type, MapType);
        return map.Select(p => new KeyValuePair<string, object?>(p.Key, p.Value(o)));
    }

    private IReadOnlyList<KeyValuePair<string, Func<object, object>>> MapType(Type type)
    {
        var map = new List<KeyValuePair<string, Func<object, object>>>();

        var fields = type.GetFields(_bindingFlags)
            .GroupBy(f => f.Name)
            .Select(GetNewest);

        foreach (var field in fields)
            map.Add(new KeyValuePair<string, Func<object, object>>(field.Name, field.GetValue));

        var props = type.GetProperties(_bindingFlags)
            .Where(x => x.CanRead && !x.GetIndexParameters().Any())
            .GroupBy(f => f.Name)
            .Select(GetNewest);

        foreach (var prop in props)
            map.Add(new KeyValuePair<string, Func<object, object>>(prop.Name, prop.GetValue));

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