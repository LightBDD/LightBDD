#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeArray : ObjectTreeNode
{
    public ObjectTreeArray(ObjectTreeNode? parent, string node) : base(parent, node)
    {
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Array;

    public IReadOnlyList<ObjectTreeNode> Items { get; internal set; }

    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
        foreach (var item in Items.SelectMany(i => i.EnumerateAll()))
            yield return item;
    }

    public override string ToString() => $"<array:{Items.Count}>";
}