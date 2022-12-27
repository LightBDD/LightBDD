#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeReference : ObjectTreeNode, ISelfFormattable
{
    public ObjectTreeReference(ObjectTreeNode? parent, string node, ObjectTreeNode target, object? rawObject) : base(parent, node, rawObject)
    {
        Target = target;
    }

    public ObjectTreeNode Target { get; }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Reference;

    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
    }

    public string Format(IValueFormattingService formattingService) => ToString();
    public override string ToString() => $"<ref: {Target.Path}>";
}