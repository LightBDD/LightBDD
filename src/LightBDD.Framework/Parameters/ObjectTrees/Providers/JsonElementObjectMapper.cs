﻿#nullable enable
using System.Collections.Generic;
using System.Text.Json;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class JsonElementObjectMapper : ObjectMapper
{
    public static readonly JsonElementObjectMapper Instance = new();
    private JsonElementObjectMapper() { }

    public override bool CanMap(object obj) => obj is JsonElement { ValueKind: JsonValueKind.Object };
    public override IEnumerable<ObjectProperty> GetProperties(object o)
    {
        var j = (JsonElement)o;
        foreach (var jsonProperty in j.EnumerateObject())
            yield return new ObjectProperty(jsonProperty.Name, jsonProperty.Value);
    }
}