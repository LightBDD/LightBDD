using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeArray : ObjectTreeNode
{
    public ObjectTreeArray(string path, IEnumerable<ObjectTreeNode> items) : base(path)
    {
        Items = items.ToArray();
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Array;
    public IReadOnlyList<ObjectTreeNode> Items { get; }
}