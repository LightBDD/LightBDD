#nullable enable
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Object map produced by <seealso cref="ObjectMapper"/>
/// </summary>
public sealed class ObjectMap
{
    /// <summary>
    /// Object properties
    /// </summary>
    public IEnumerable<ObjectProperty> Properties { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="properties"></param>
    public ObjectMap(IEnumerable<ObjectProperty> properties)
    {
        Properties = properties;
    }
}