#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class JsonElementValueMapper : ValueMapper
{
    public static readonly JsonElementValueMapper Instance = new();
    private JsonElementValueMapper() { }

    public override bool CanMap(object obj) => obj is JsonElement e && e.ValueKind != JsonValueKind.Object && e.ValueKind != JsonValueKind.Array;
    public override object? GetValue(object o)
    {
        var j = (JsonElement)o;
        switch (j.ValueKind)
        {
            case JsonValueKind.String:
                return j.GetString();
            case JsonValueKind.Number:
                if (j.TryGetInt32(out var i32))
                    return i32;
                if (j.TryGetInt64(out var i64))
                    return i64;
                return j.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                throw new NotSupportedException($"{j.ValueKind} is not supported");
        }
    }
}