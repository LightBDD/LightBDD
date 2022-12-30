#nullable enable
using System;
using System.Text.Json;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Value mapper for <seealso cref="JsonElement"/> containing value element.
/// </summary>
public class JsonElementValueMapper : ValueMapper
{
    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly JsonElementValueMapper Instance = new();

    private JsonElementValueMapper() { }

    /// <summary>
    /// Returns true if <paramref name="obj"/> is <seealso cref="JsonElement"/> containing value element.
    /// </summary>
    public override bool CanMap(object obj) => obj is JsonElement e && e.ValueKind != JsonValueKind.Object && e.ValueKind != JsonValueKind.Array;

    /// <summary>
    /// Returns value of JsonElement, where method supports <seealso cref="JsonValueKind.String"/>, <seealso cref="JsonValueKind.Number"/>, <seealso cref="JsonValueKind.True"/>, <seealso cref="JsonValueKind.False"/>, <seealso cref="JsonValueKind.Null"/> value kinds.<br/>
    /// For <seealso cref="JsonValueKind.Number"/>, it returns <seealso cref="int"/>,<seealso cref="long"/> or <seealso cref="double"/> types, depending on number type and size.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown for unsupported kinds.</exception>
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