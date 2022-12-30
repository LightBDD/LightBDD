#nullable enable
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class ExpandoObjectMapper : ObjectMapper
{
    public static readonly ExpandoObjectMapper Instance = new();
    private ExpandoObjectMapper() { }

    public override bool CanMap(object obj) => obj is ExpandoObject;
    public override IEnumerable<ObjectProperty> GetProperties(object o)
    {
        return ((IDictionary<string, object?>)o).Select(p => new ObjectProperty(p.Key, p.Value));
    }
}