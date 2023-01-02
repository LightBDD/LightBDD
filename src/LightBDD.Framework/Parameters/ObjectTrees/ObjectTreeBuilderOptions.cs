#nullable enable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
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

    private ImmutableHashSet<Type> _unorderedCollectionTypes = ImmutableHashSet.Create(
        typeof(HashSet<>),
        typeof(ImmutableHashSet<>),
        typeof(ConcurrentBag<>),
        typeof(Dictionary<,>),
        typeof(ConcurrentDictionary<,>),
        typeof(ReadOnlyDictionary<,>),
        typeof(ImmutableDictionary<,>)
    );

    private ImmutableStack<NodeMapper> _mappers = ImmutableStack.Create(new NodeMapper[]{
        PlainObjectMapper.Instance,
        EnumerableArrayMapper.Instance,
        UnorderedArrayMapper.Instance,
        StringDictionaryObjectMapper.Instance,
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
    /// List of types that would be treated as unordered collection, thus if possible, the mappers will sort their items during map.
    /// </summary>
    public IReadOnlyCollection<Type> UnorderedCollectionTypes => _unorderedCollectionTypes;

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
    /// Adds <paramref name="type"/> to <seealso cref="UnorderedCollectionTypes"/>. The method accepts open generic types, i.e. <c>IEnumerable&lt;&gt;</c>
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if provided type does not implement <c>IEnumerable</c></exception>
    public ObjectTreeBuilderOptions AddUnorderedCollectionType(Type type)
    {
        if (!typeof(IEnumerable).IsAssignableFrom(type))
            throw new InvalidOperationException($"Type {type} has to implement {typeof(IEnumerable)}");

        var clone = Clone();
        clone._unorderedCollectionTypes = _unorderedCollectionTypes.Add(type);
        return clone;
    }

    /// <summary>
    /// Removes <paramref name="type"/> from <seealso cref="UnorderedCollectionTypes"/>.
    /// </summary>
    public ObjectTreeBuilderOptions RemoveUnorderedCollectionType(Type type)
    {
        var clone = Clone();
        clone._unorderedCollectionTypes = _unorderedCollectionTypes.Remove(type);
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