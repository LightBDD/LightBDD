using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Parameters.ObjectTrees.Providers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeBuilder
{
    private readonly ObjectTreeBuilderOptions _options;

    public ObjectTreeBuilder(ObjectTreeBuilderOptions options)
    {
        _options = options;
    }

    public ObjectTreeNode Build(object? o) => Build(o, CombinePath(null));

    private ObjectTreeNode Build(object? o, string path)
    {
        if (o is null)
            return new ObjectTreeValue(path, o);

        var type = o.GetType();
        if (type.IsPrimitive || _options.ValueTypes.Any(t => t.IsAssignableFrom(type)))
            return new ObjectTreeValue(path, o);

        var collection = _options.ArrayProviders
            .Select(p => p.TryProvide(o))
            .FirstOrDefault(r => r != null);

        if (collection != null)
            return CreateArray(collection, path);

        var properties = _options.ObjectProviders
            .Select(p => p.TryProvide(o))
            .FirstOrDefault(r => r != null);

        if (properties != null)
            return CreateObject(properties, path);
        return CreateObject(PocoObjectProvider.Instance.Provide(o), path);
    }

    private ObjectTreeNode CreateObject(IEnumerable<KeyValuePair<string, object>> properties, string path)
    {
        var props = properties.ToDictionary(x => x.Key, x => Build(x.Value, CombinePath(path, $".{x.Key}")));
        return new ObjectTreeObject(path, props);
    }

    private ObjectTreeArray CreateArray(IEnumerable collection, string path)
    {
        var items = collection.Cast<object>().Select((o, i) => Build(o, CombinePath(path, $"[{i}]")));
        return new ObjectTreeArray(path, items);
    }

    private static string CombinePath(string? parent, string? node = null)
    {
        parent ??= "$";
        return string.IsNullOrWhiteSpace(node)
            ? parent
            : $"{parent}{node}";
    }
}