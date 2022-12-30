#nullable enable
using System;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Node mapper allowing to map input objects into tree nodes
/// </summary>
public abstract class NodeMapper
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="kind">Type of nodes this mapper supports.</param>
    protected NodeMapper(ObjectTreeNodeKind kind)
    {
        Kind = kind;
    }

    /// <summary>
    /// Type of nodes this mapper supports.
    /// </summary>
    public ObjectTreeNodeKind Kind { get; }

    /// <summary>
    /// Returns true if the object can be mapped
    /// </summary>
    public abstract bool CanMap(object obj);

    /// <summary>
    /// Returns <seealso cref="ArrayMapper"/> representation of this mapper if mapper is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if mapper is not of this type.</exception>
    public ArrayMapper AsArrayMapper() => this as ArrayMapper ?? throw new InvalidOperationException($"{GetType().Name} is not an {nameof(ArrayMapper)}");

    /// <summary>
    /// Returns <seealso cref="ObjectMapper"/> representation of this mapper if mapper is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if mapper is not of this type.</exception>
    public ObjectMapper AsObjectMapper() => this as ObjectMapper ?? throw new InvalidOperationException($"{GetType().Name} is not an {nameof(ArrayMapper)}");

    /// <summary>
    /// Returns <seealso cref="ValueMapper"/> representation of this mapper if mapper is of this type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if mapper is not of this type.</exception>
    public ValueMapper AsValueMapper() => this as ValueMapper ?? throw new InvalidOperationException($"{GetType().Name} is not an {nameof(ValueMapper)}");
}