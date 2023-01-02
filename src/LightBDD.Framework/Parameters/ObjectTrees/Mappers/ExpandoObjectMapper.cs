#nullable enable
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Mapper for <seealso cref="ExpandoObject"/> that represents it as object node.
/// </summary>
public class ExpandoObjectMapper : ObjectMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly ExpandoObjectMapper Instance = new();
    private ExpandoObjectMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="ExpandoObject"/>
    /// </summary>
    public override bool CanMap(object obj) => obj is ExpandoObject;
    /// <summary>
    /// Returns properties of expando object.
    /// </summary>
    public override ObjectMap MapObject(object o)
    {
        return new ObjectMap(((IDictionary<string, object?>)o).Select(p => new ObjectProperty(p.Key, p.Value)));
    }
}