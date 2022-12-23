using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeObject : ObjectTreeNode
{
    public ObjectTreeObject(string path, IReadOnlyDictionary<string, ObjectTreeNode> properties) : base(path)
    {
        Properties = properties;
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Object;

    public IReadOnlyDictionary<string, ObjectTreeNode> Properties { get; }

    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
        foreach (var node in Properties.Values.SelectMany(v => v.EnumerateAll()))
            yield return node;
    }

    public override string ToString() => "<object>";
}