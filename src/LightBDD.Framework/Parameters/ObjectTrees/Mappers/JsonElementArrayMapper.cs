#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Array mapper for <seealso cref="JsonElement"/> of <seealso cref="JsonValueKind.Array"/> kind.
/// </summary>
public class JsonElementArrayMapper : ArrayMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly JsonElementArrayMapper Instance = new();

    private JsonElementArrayMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="JsonElement"/> of <seealso cref="JsonValueKind.Array"/> kind.
    /// </summary>
    public override bool CanMap(object obj, ObjectTreeBuilderOptions options) => obj is JsonElement { ValueKind: JsonValueKind.Array };

    /// <summary>
    /// Maps JsonElement to array
    /// </summary>
    public override ArrayMap MapArray(object o, ObjectTreeBuilderOptions options)
    {
        return new ArrayMap(GetItems(o));
    }
    private static IEnumerable<object?> GetItems(object o)
    {
        return ((JsonElement)o).EnumerateArray().Cast<object>();
    }
}