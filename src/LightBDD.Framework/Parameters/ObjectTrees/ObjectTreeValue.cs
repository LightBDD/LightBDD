#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeValue : ObjectTreeNode, ISelfFormattable
{
    public ObjectTreeValue(ObjectTreeNode? parent, string node, object? value) : base(parent, node)
    {
        Value = value;
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Value;

    public object? Value { get; }

    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
    }

    public string Format(IValueFormattingService formattingService) => formattingService.FormatValue(Value);
}