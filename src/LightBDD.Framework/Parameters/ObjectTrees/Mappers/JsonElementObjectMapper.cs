#nullable enable
using System.Collections.Generic;
using System.Text.Json;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Object mapper for <seealso cref="JsonElement"/> of <seealso cref="JsonValueKind.Object"/> kind.
/// </summary>
public class JsonElementObjectMapper : ObjectMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly JsonElementObjectMapper Instance = new();

    private JsonElementObjectMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="JsonElement"/> of <seealso cref="JsonValueKind.Object"/> kind.
    /// </summary>
    public override bool CanMap(object obj) => obj is JsonElement { ValueKind: JsonValueKind.Object };

    /// <summary>
    /// Returns properties of JsonElement object.
    /// </summary>
    public override IEnumerable<ObjectProperty> GetProperties(object o)
    {
        var j = (JsonElement)o;
        foreach (var jsonProperty in j.EnumerateObject())
            yield return new ObjectProperty(jsonProperty.Name, jsonProperty.Value);
    }
}