#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Represents array node of object tree.
/// </summary>
public class ObjectTreeArray : ObjectTreeNode
{
    internal ObjectTreeArray(ObjectTreeNode? parent, string node, object rawObject) : base(parent, node, rawObject)
    {
    }

    /// <summary>
    /// Returns <seealso cref="ObjectTreeNodeKind.Array"/>
    /// </summary>
    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Array;

    /// <summary>
    /// Returns array item nodes.
    /// </summary>
    public IReadOnlyList<ObjectTreeNode> Items { get; internal set; } = Array.Empty<ObjectTreeNode>();

    /// <inheritdoc />
    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
        foreach (var item in Items.SelectMany(i => i.EnumerateAll()))
            yield return item;
    }

    /// <inheritdoc />
    public override string ToString() => $"<array:{Items.Count}>";
}