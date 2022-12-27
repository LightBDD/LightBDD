#nullable enable
using System.Collections.Generic;
using System.Dynamic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class ExpandoMapper : ObjectMapper
{
    public static readonly ExpandoMapper Instance = new();
    private ExpandoMapper() { }

    public override bool CanMap(object obj) => obj is ExpandoObject;
    public override IEnumerable<KeyValuePair<string, object>> GetProperties(object o)
    {
        return ((IDictionary<string, object?>)o);
    }
}