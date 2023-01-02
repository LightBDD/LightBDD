#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using LightBDD.Framework.Parameters.ObjectTrees.Mappers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

/// <summary>
/// Represents <seealso cref="ObjectTreeBuilder"/> options. This class implements immutable pattern, thus any alterations to the options creates a new instance.
/// </summary>
public class ObjectTreeBuilderOptions
{
    /// <summary>
    /// Default options
    /// </summary>
    public static readonly ObjectTreeBuilderOptions Default = new();

    private ImmutableHashSet<Type> _valueTypes = ImmutableHashSet.Create(
        typeof(string),
        typeof(MemberInfo) //when traversing through exceptions
    );

    private ImmutableStack<NodeMapper> _mappers = ImmutableStack.Create(new NodeMapper[]{
        PocoMapper.Instance,
        EnumerableMapper.Instance,
        ExpandoObjectMapper.Instance,
        FormattableValueMapper.Instance,
        JsonElementObjectMapper.Instance,
        JsonElementArrayMapper.Instance,
        JsonElementValueMapper.Instance,
        ExpectationValueMapper.Instance});

    private int _maxDepth = 32;

    /// <summary>
    /// List of types that would be mapped as value nodes. Types present on this list will not be evaluated against mappers defined on <seealso cref="Mappers"/> collection.
    /// </summary>
    public IReadOnlyCollection<Type> ValueTypes => _valueTypes;

    /// <summary>
    /// Collection of mappers to be used for object->node mapping. The order of mappers execution is inverted, i.e. mapper added as last one will be used as first one, which allows to register the most specific mappers as first ones.
    /// </summary>
    public IEnumerable<NodeMapper> Mappers => _mappers;

    /// <summary>
    /// Maximum depth of nodes. Default value: 32
    /// </summary>
    public int MaxDepth => _maxDepth;

    /// <summary>
    /// Adds <paramref name="type"/> to <seealso cref="ValueTypes"/>.
    /// </summary>
    public ObjectTreeBuilderOptions AddValueType(Type type)
    {
        var clone = Clone();
        clone._valueTypes = _valueTypes.Add(type);
        return clone;
    }

    /// <summary>
    /// Removes <paramref name="type"/> from <seealso cref="ValueTypes"/>.
    /// </summary>
    public ObjectTreeBuilderOptions RemoveValueType(Type type)
    {
        var clone = Clone();
        clone._valueTypes = _valueTypes.Remove(type);
        return clone;
    }

    /// <summary>
    /// Sets <seealso cref="MaxDepth"/> to <paramref name="maxDepth"/>.
    /// </summary>
    public ObjectTreeBuilderOptions WithMaxDepth(int maxDepth)
    {
        var clone = Clone();
        clone._maxDepth = maxDepth;
        return clone;
    }

    /// <summary>
    /// Appends <paramref name="mapper"/> to <seealso cref="Mappers"/>.
    /// </summary>
    public ObjectTreeBuilderOptions AppendMapper(NodeMapper mapper)
    {
        var clone = Clone();
        clone._mappers = _mappers.Push(mapper);
        return clone;
    }

    /// <summary>
    /// Clears <seealso cref="Mappers"/>.
    /// </summary>
    public ObjectTreeBuilderOptions ClearMappers()
    {
        var clone = Clone();
        clone._mappers = _mappers.Clear();
        return clone;
    }

    private ObjectTreeBuilderOptions() { }
    private ObjectTreeBuilderOptions Clone() => (ObjectTreeBuilderOptions)MemberwiseClone();
}