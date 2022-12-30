#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Execution.Coordination;
using LightBDD.Framework.Parameters.ObjectTrees.Mappers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Class allowing to build object tree representation of input object.
/// </summary>
public class ObjectTreeBuilder
{
    private readonly ObjectTreeBuilderOptions _options;

    /// <summary>
    /// Default instance.
    /// </summary>
    public static ObjectTreeBuilder Default { get; } = new(new());

    /// <summary>
    /// Currently configured instance.
    /// </summary>
    public static ObjectTreeBuilder Current => FrameworkFeatureCoordinator.TryGetInstance()?.Configuration.Get<ObjectTreeConfiguration>().Builder ?? Default;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="options">Options</param>
    public ObjectTreeBuilder(ObjectTreeBuilderOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Builds object tree representation of provided <paramref name="o"/> object and returns root node.
    /// </summary>
    public ObjectTreeNode Build(object? o) => Build(o, "$", null);

    private ObjectTreeNode Build(object? o, string node, ObjectTreeNode? parent)
    {
        if (o is null)
            return new ObjectTreeValue(parent, node, o, o);

        var recursionTarget = FindRecursion(o, parent);
        if (recursionTarget != null)
            return new ObjectTreeReference(parent, node, recursionTarget, o);

        var type = o.GetType();
        if (type.IsPrimitive || _options.ValueTypes.Any(t => IsImplementingType(type, t)))
            return new ObjectTreeValue(parent, node, o, o);

        try
        {
            var mapper = _options.Mappers.FirstOrDefault(m => m.CanMap(o));
            switch (mapper?.Kind)
            {
                case ObjectTreeNodeKind.Value:
                    return new ObjectTreeValue(parent, node, mapper.AsValueMapper().GetValue(o), o);
                case ObjectTreeNodeKind.Array:
                    return CreateArray(mapper.AsArrayMapper().GetItems(o), node, parent, o);
                case ObjectTreeNodeKind.Object:
                    return CreateObject(mapper.AsObjectMapper().GetProperties(o), node, parent, o);
            }

            return CreateObject(PocoMapper.Instance.GetProperties(o), node, parent, o);
        }
        catch (Exception ex)
        {
            return new ObjectTreeValue(parent, node, new ExceptionCapture(ex), o);
        }
    }

    private static bool IsImplementingType(Type type, Type target)
    {
        if (!target.IsGenericTypeDefinition)
            return target.IsAssignableFrom(type);

        if (target.IsInterface)
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == target);

        var t = type;
        while (t != null)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == target)
                return true;
            t = t.BaseType;
        }

        return false;
    }

    private static ObjectTreeNode? FindRecursion(object o, ObjectTreeNode? node)
    {
        while (node != null)
        {
            if (ReferenceEquals(node.RawObject, o))
                return node;
            node = node.Parent;
        }

        return null;
    }

    private ObjectTreeNode CreateObject(IEnumerable<ObjectProperty> properties, string node, ObjectTreeNode? parent, object o)
    {
        var result = new ObjectTreeObject(parent, node, o);
        result.Properties = properties.ToDictionary(x => x.Name, x => Build(x.Value, GetNodePath(x.Name), result));
        return result;
    }

    private ObjectTreeArray CreateArray(IEnumerable collection, string node, ObjectTreeNode? parent, object o)
    {
        var result = new ObjectTreeArray(parent, node, o);
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