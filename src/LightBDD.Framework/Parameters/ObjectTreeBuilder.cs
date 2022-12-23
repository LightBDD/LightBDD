#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters;

public class ObjectTreeBuilderOptions
{
    public HashSet<Type> ValueTypes { get; } = new()
    {
        typeof(string),
        typeof(IFormattable)
    };

}
public class ObjectTreeBuilder
{
    private readonly ObjectTreeBuilderOptions _options;

    public ObjectTreeBuilder(ObjectTreeBuilderOptions options)
    {
        _options = options;
    }

    public ObjectTreeNode Build(object? o) => Build(o, CombinePath(null));

    private ObjectTreeNode Build(object? o, string path)
    {
        if (o is null)
            return new ObjectTreeValue(path, o);

        var type = o.GetType();
        if (type.IsPrimitive || _options.ValueTypes.Any(t => t.IsAssignableFrom(type)))
            return new ObjectTreeValue(path, o);

        if (o is IEnumerable collection)
            return CreateArray(collection, path);

        return CreateObject(o, type, path);
    }

    private ObjectTreeNode CreateObject(object o, Type type, string path)
    {
        var props = type.GetFields().Select(f => new KeyValuePair<string, object?>(f.Name, f.GetValue(o)))
            .Concat(type.GetProperties().Select(p => new KeyValuePair<string, object?>(p.Name, p.GetValue(o))))
            .ToDictionary(x => x.Key, x => Build(x.Value, CombinePath(path, $".{x.Key}")));
        return new ObjectTreeObject(path, props);
    }

    private ObjectTreeArray CreateArray(IEnumerable collection, string path)
    {
        var items = collection.Cast<object>().Select((o, i) => Build(o, CombinePath(path, $"[{i}]")));
        return new ObjectTreeArray(path, items);
    }

    private static string CombinePath(string? parent, string? node = null)
    {
        parent ??= "$";
        return string.IsNullOrWhiteSpace(node)
            ? parent
            : $"{parent}{node}";
    }
}

public abstract class ObjectTreeNode
{
    public string Path { get; }
    public abstract ObjectTreeNodeKind Kind { get; }

    protected ObjectTreeNode(string path)
    {
        Path = path;
    }

    public ObjectTreeValue AsValue() => this as ObjectTreeValue ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeValue)}");
    public ObjectTreeArray AsArray() => this as ObjectTreeArray ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeArray)}");
    public ObjectTreeObject AsObject() => this as ObjectTreeObject ?? throw new InvalidOperationException($"{GetType().Name} node at {Path} path is not {nameof(ObjectTreeObject)}");
}

public class ObjectTreeObject : ObjectTreeNode
{
    public ObjectTreeObject(string path, IReadOnlyDictionary<string, ObjectTreeNode> properties) : base(path)
    {
        Properties = properties;
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Object;
    public IReadOnlyDictionary<string, ObjectTreeNode> Properties { get; }
}

public class ObjectTreeArray : ObjectTreeNode
{
    public ObjectTreeArray(string path, IEnumerable<ObjectTreeNode> items) : base(path)
    {
        Items = items.ToArray();
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Array;
    public IReadOnlyList<ObjectTreeNode> Items { get; }
}

public class ObjectTreeValue : ObjectTreeNode
{
    public ObjectTreeValue(string path, object? value) : base(path)
    {
        Value = value;
    }

    public override ObjectTreeNodeKind Kind => ObjectTreeNodeKind.Value;
    public object? Value { get; }
}

public enum ObjectTreeNodeKind
{
    Value, Array, Object
}