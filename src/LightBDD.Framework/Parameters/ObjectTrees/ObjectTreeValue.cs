namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeValue : ObjectTreeNode
{
    public ObjectTreeValue(string path, object? value) : base(path)
    {
        Value = value;
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Value;
    public object? Value { get; }
}