#nullable enable
using System;
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public abstract class ObjectTreeNode
{
    public ObjectTreeNode? Parent { get; }
    public string Node { get; }
    public string Path { get; }
    public abstract ObjectTreeNodeKind Kind { get; }

    protected ObjectTreeNode(ObjectTreeNode? parent, string node)
    {
        Parent = parent;
        Node = node;
        Path = parent != null ? node.StartsWith("[") ? $"{parent.Path}{node}" : $"{parent.Path}.{node}" : node;
    }

    public ObjectTreeValue AsValue() => this as ObjectTreeValue ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeValue)}");
    public ObjectTreeArray AsArray() => this as ObjectTreeArray ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeArray)}");
    public ObjectTreeObject AsObject() => this as ObjectTreeObject ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeObject)}");

    public abstract IEnumerable<ObjectTreeNode> EnumerateAll();
}