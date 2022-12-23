using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeValue : ObjectTreeNode,ISelfFormattable
{
    public ObjectTreeValue(string path, object? value) : base(path)
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