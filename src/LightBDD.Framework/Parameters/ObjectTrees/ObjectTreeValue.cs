#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Formatting.Values;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Represents value node of object tree.
/// </summary>
public class ObjectTreeValue : ObjectTreeNode, ISelfFormattable
{
    internal ObjectTreeValue(ObjectTreeNode? parent, string node, object? value, object? rawObject) : base(parent, node, rawObject)
    {
        Value = value;
    }

    /// <summary>
    /// Returns <seealso cref="ObjectTreeNodeKind.Value"/>
    /// </summary>
    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Value;

    /// <summary>
    /// Node value. Unlike <seealso cref="ObjectTreeNode.RawObject"/>, this value may contain value instance that has been transformed by one of mappers defined in <seealso cref="ObjectTreeBuilderOptions.Mappers"/>.
    /// </summary>
    public object? Value { get; }

    /// <inheritdoc />
    public override IEnumerable<ObjectTreeNode> EnumerateAll()
    {
        yield return this;
    }

    /// <inheritdoc />
    public string Format(IValueFormattingService formattingService) => formattingService.FormatValue(Value);

    /// <inheritdoc />
    public override string ToString() => Format(ValueFormattingServices.Current);
}