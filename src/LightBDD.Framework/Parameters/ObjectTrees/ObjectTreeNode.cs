using System;
using System.Collections;
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public abstract class ObjectTreeNode
{
    public string Path { get; }
    public abstract ObjectTreeNodeKind Kind { get; }

    protected ObjectTreeNode(string path)
    {
        Path = path;
    }

    public ObjectTreeValue AsValue() => this as ObjectTreeValue ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeValue)}");
    public ObjectTreeArray AsArray() => this as ObjectTreeArray ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeArray)}");
    public ObjectTreeObject AsObject() => this as ObjectTreeObject ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeObject)}");

    public abstract IEnumerable<ObjectTreeNode> EnumerateAll();
}