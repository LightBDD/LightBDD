#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeObject : ObjectTreeNode
{
    internal ObjectTreeObject(ObjectTreeNode? parent, string node) : base(parent, node)
    {
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Object;

    public IReadOnlyDictionary<string, ObjectTreeNode> Properties { get; internal set; }

    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
        foreach (var node in Properties.Values.SelectMany(v => v.EnumerateAll()))
            yield return node;
    }

    public override string ToString() => "<object>";
}