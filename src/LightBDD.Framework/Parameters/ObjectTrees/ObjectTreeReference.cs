#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Represent reference node of object tree, that can point to already existing node in the upper part of the object graph - used to represent circular references
/// </summary>
public class ObjectTreeReference : ObjectTreeNode, ISelfFormattable
{
    internal ObjectTreeReference(ObjectTreeNode? parent, string node, ObjectTreeNode target, object? rawObject) : base(parent, node, rawObject)
    {
        Target = target;
    }

    /// <summary>
    /// Target node
    /// </summary>
    public ObjectTreeNode Target { get; }

    /// <summary>
    /// Returns <seealso cref="ObjectTreeNodeKind.Reference"/>
    /// </summary>
    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Reference;

    /// <inheritdoc />
    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
    }

    string ISelfFormattable.Format(IValueFormattingService formattingService) => ToString();

    /// <inheritdoc />
    public override string ToString() => $"<ref: {Target.Path}>";
}