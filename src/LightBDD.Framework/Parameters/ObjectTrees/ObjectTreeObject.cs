#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Represents object node of object tree.
/// </summary>
public class ObjectTreeObject : ObjectTreeNode
{
    private static readonly IReadOnlyDictionary<string, ObjectTreeNode> EmptyDictionary = new ReadOnlyDictionary<string, ObjectTreeNode>(new Dictionary<string, ObjectTreeNode>());

    internal ObjectTreeObject(ObjectTreeNode? parent, string node, object rawObject) : base(parent, node, rawObject)
    {
    }

    /// <summary>
    /// Returns <seealso cref="ObjectTreeNodeKind.Object"/>
    /// </summary>
    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Object;

    /// <summary>
    /// Returns object property nodes.
    /// </summary>
    public IReadOnlyDictionary<string, ObjectTreeNode> Properties { get; internal set; } = EmptyDictionary;

    /// <inheritdoc />
    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
        foreach (var node in Properties.Values.OrderBy(p => p.Node).SelectMany(v => v.EnumerateAll()))
            yield return node;
    }

    /// <inheritdoc />
    public override string ToString() => "<object>";
}