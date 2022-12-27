#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Parameters.ObjectTrees.Providers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeBuilder
{
    private readonly ObjectTreeBuilderOptions _options;

    public static ObjectTreeBuilder Default { get; } = new(new());
    public static ObjectTreeBuilder Current => FrameworkFeatureCoordinator.TryGetInstance()?.Configuration.Get<ObjectTreeConfiguration>().Builder ?? Default;


    public ObjectTreeBuilder(ObjectTreeBuilderOptions options)
    {
        _options = options;
    }

    public ObjectTreeNode Build(object? o) => Build(o, "$", null);

    private ObjectTreeNode Build(object? o, string node, ObjectTreeNode? parent)
    {
        if (o is null)
            return new ObjectTreeValue(parent, node, o);

        var type = o.GetType();
        if (type.IsPrimitive || _options.ValueTypes.Any(t => t.IsAssignableFrom(type)))
            return new ObjectTreeValue(parent, node, o);

        var mapper = _options.Mappers.FirstOrDefault(m => m.CanMap(o));
        switch (mapper?.Kind)
        {
            case ObjectTreeNodeKind.Value:
                return new ObjectTreeValue(parent, node, mapper.AsValueMapper().GetValue(o));
            case ObjectTreeNodeKind.Array:
                return CreateArray(mapper.AsArrayMapper().GetItems(o), node, parent);
            case ObjectTreeNodeKind.Object:
                return CreateObject(mapper.AsObjectMapper().GetProperties(o), node, parent);
        }
        return CreateObject(PocoMapper.Instance.GetProperties(o), node, parent);
    }

    private ObjectTreeNode CreateObject(IEnumerable<KeyValuePair<string, object?>> properties, string node, ObjectTreeNode? parent)
    {
        var result = new ObjectTreeObject(parent, node);
        result.Properties = properties.ToDictionary(x => x.Key, x => Build(x.Value, GetNodePath(x.Key), result));
        return result;
    }

    private ObjectTreeArray CreateArray(IEnumerable collection, string node, ObjectTreeNode? parent)
    {
        var result = new ObjectTreeArray(parent, node);
        result.Items = collection.Cast<object>().Select((o, i) => Build(o, GetNodePath(i), result)).ToArray();
        return result;
    }

    private static string GetNodePath(int i) => $"[{i}]";
    private static string GetNodePath(string node)
    {
        if (node.All(c => c is '_' or >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9'))
            return node;
        return $"['{node.Replace("\\", "\\\\").Replace("'", "\\'")}']";
    }
}