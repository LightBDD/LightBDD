#nullable enable
using System;
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Represents node of object tree
/// </summary>
public abstract class ObjectTreeNode
{
    /// <summary>
    /// Parent node or <c>null</c> if this node is root.
    /// </summary>
    public ObjectTreeNode? Parent { get; }
    /// <summary>
    /// Node name
    /// </summary>
    public string Node { get; }
    /// <summary>
    /// Raw object that this node represents.
    /// </summary>
    public object? RawObject { get; }
    /// <summary>
    /// Node path in the object tree
    /// </summary>
    public string Path { get; }
    /// <summary>
    /// Specifies kind of the node.
    /// </summary>
    public abstract ObjectTreeNodeKind Kind { get; }
    /// <summary>
    /// Node depth in the object tree
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="node">Node name</param>
    /// <param name="rawObject">Raw object</param>
    protected ObjectTreeNode(ObjectTreeNode? parent, string node, object? rawObject)
    {
        Parent = parent;
        Node = node;
        Depth = (parent?.Depth ?? 0) + 1;
        RawObject = rawObject;
        Path = parent != null ? node.StartsWith("[") ? $"{parent.Path}{node}" : $"{parent.Path}.{node}" : node;
    }

    /// <summary>
    /// Returns <seealso cref="ObjectTreeValue"/> representation of this node if it is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if node is not of this type.</exception>
    public ObjectTreeValue AsValue() => this as ObjectTreeValue ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeValue)}");
    /// <summary>
    /// Returns <seealso cref="ObjectTreeArray"/> representation of this node if it is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if node is not of this type.</exception>
    public ObjectTreeArray AsArray() => this as ObjectTreeArray ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeArray)}");
    /// <summary>
    /// Returns <seealso cref="ObjectTreeObject"/> representation of this node if it is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if node is not of this type.</exception>
    public ObjectTreeObject AsObject() => this as ObjectTreeObject ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeObject)}");
    /// <summary>
    /// Returns <seealso cref="ObjectTreeReference"/> representation of this node if it is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if node is not of this type.</exception>
    public ObjectTreeReference AsReference() => this as ObjectTreeReference ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeReference)}");

    /// <summary>
    /// Checks if this node has an associated exception and returns it.
    /// </summary>
    public bool HasExceptionCaptured(out Exception? exception)
    {
        exception = null;
        if (Kind != ObjectTreeNodeKind.Value || AsValue().Value is not ExceptionCapture c)
            return false;
        exception = c.Exception;
        return true;
    }

    /// <summary>
    /// Enumerates all nodes, which includes self and all descendants.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<ObjectTreeNode> EnumerateAll();
}