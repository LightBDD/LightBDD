#nullable enable
using System;
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
    /// <summary>
    /// Default instance.
    /// </summary>
    public static readonly ObjectTreeBuilder Default = new(ObjectTreeBuilderOptions.Default);

    /// <summary>
    /// Currently configured instance.
    /// </summary>
    public static ObjectTreeBuilder Current => FrameworkFeatureCoordinator.TryGetInstance()?.Configuration.Get<ObjectTreeConfiguration>().Builder ?? Default;

    /// <summary>
    /// Builder options.
    /// </summary>
    public ObjectTreeBuilderOptions Options { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="options">Options</param>
    public ObjectTreeBuilder(ObjectTreeBuilderOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Builds object tree representation of provided <paramref name="o"/> object and returns root node.
    /// </summary>
    public ObjectTreeNode Build(object? o) => Build(o, "$", null);

    private ObjectTreeNode Build(object? o, string node, ObjectTreeNode? parent)
    {
        try
        {
            if (parent?.Depth >= Options.MaxDepth)
                throw new InvalidOperationException("Maximum node depth reached");

            if (o is null)
                return new ObjectTreeValue(parent, node, o, o);

            var recursionTarget = FindRecursion(o, parent);
            if (recursionTarget != null)
                return new ObjectTreeReference(parent, node, recursionTarget, o);

            var type = o.GetType();
            if (type.IsPrimitive || Options.ValueTypes.Any(t => IsImplementingType(type, t)))
                return new ObjectTreeValue(parent, node, o, o);


            var mapper = Options.Mappers.FirstOrDefault(m => m.CanMap(o));
            switch (mapper?.Kind)
            {
                case ObjectTreeNodeKind.Value:
                    return new ObjectTreeValue(parent, node, mapper.AsValueMapper().MapValue(o), o);
                case ObjectTreeNodeKind.Array:
                    return CreateArray(mapper.AsArrayMapper().MapArray(o), node, parent, o);
                case ObjectTreeNodeKind.Object:
                    return CreateObject(mapper.AsObjectMapper().MapObject(o), node, parent, o);
            }

            return CreateObject(PocoMapper.Instance.MapObject(o), node, parent, o);
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

    private ObjectTreeNode CreateObject(ObjectMap map, string node, ObjectTreeNode? parent, object o)
    {
        var result = new ObjectTreeObject(parent, node, o);
        result.Properties = map.Properties.ToDictionary(x => x.Name, x => Build(x.Value, GetNodePath(x.Name), result));
        return result;
    }

    private ObjectTreeArray CreateArray(ArrayMap map, string node, ObjectTreeNode? parent, object o)
    {
        var result = new ObjectTreeArray(parent, node, o);
        result.Items = map.Items.Cast<object>().Select((o, i) => Build(o, GetNodePath(i), result)).ToArray();
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