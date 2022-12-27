#nullable enable
using System;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

/// <summary>
/// Node mapper allowing to map input objects into tree nodes
/// </summary>
public abstract class NodeMapper
{
    protected NodeMapper(ObjectTreeNodeKind kind)
    {
        Kind = kind;
    }

    public ObjectTreeNodeKind Kind { get; }

    /// <summary>
    /// Returns true if the object can be mapped
    /// </summary>
    public abstract bool CanMap(object obj);

    public ArrayMapper AsArrayMapper() => this as ArrayMapper ?? throw new InvalidOperationException($"{GetType().Name} is not an {nameof(ArrayMapper)}");
    public ObjectMapper AsObjectMapper() => this as ObjectMapper ?? throw new InvalidOperationException($"{GetType().Name} is not an {nameof(ArrayMapper)}");
    public ValueMapper AsValueMapper() => this as ValueMapper ?? throw new InvalidOperationException($"{GetType().Name} is not an {nameof(ValueMapper)}");
}