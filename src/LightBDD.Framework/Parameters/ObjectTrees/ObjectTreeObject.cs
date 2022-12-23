using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeObject : ObjectTreeNode
{
    public ObjectTreeObject(string path, IReadOnlyDictionary<string, ObjectTreeNode> properties) : base(path)
    {
        Properties = properties;
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Object;
    public IReadOnlyDictionary<string, ObjectTreeNode> Properties { get; }
}