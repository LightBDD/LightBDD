#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class PocoObjectProvider : IObjectProvider
{
    public static readonly PocoObjectProvider Instance = new();
    private PocoObjectProvider() { }

    public IEnumerable<KeyValuePair<string, object?>> Provide(object o)
    {
        var type = o.GetType();
        return type.GetFields().Select(f => new KeyValuePair<string, object?>(f.Name, f.GetValue(o)))
            .Concat(type.GetProperties().Select(p => new KeyValuePair<string, object?>(p.Name, p.GetValue(o))));
    }

    IEnumerable<KeyValuePair<string, object?>>? IObjectProvider.TryProvide(object o) => Provide(o);
}