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
    public override bool CanMap(object obj) => obj is JsonElement { ValueKind: JsonValueKind.Array };

    /// <summary>
    /// Returns items of JsonElement array
    /// </summary>
    public override IEnumerable<object?> GetItems(object o)
    {
        return ((JsonElement)o).EnumerateArray().Cast<object>();
    }
}