#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class JsonElementArrayMapper : ArrayMapper
{
    public static readonly JsonElementArrayMapper Instance = new();
    private JsonElementArrayMapper() { }

    public override bool CanMap(object obj) => obj is JsonElement { ValueKind: JsonValueKind.Array };
    public override IEnumerable<object?> GetItems(object o)
    {
        return ((JsonElement)o).EnumerateArray().Cast<object>();
    }
}