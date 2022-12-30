#nullable enable
using System;
using System.Collections.Generic;
using LightBDD.Framework.Parameters.ObjectTrees.Providers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeBuilderOptions
{
    /// <summary>
    /// List of types that would be mapped as value nodes. Types present on this list will not be evaluated against mappers defined on <seealso cref="Mappers"/> collection.
    /// </summary>
    public HashSet<Type> ValueTypes { get; } = new()
    {
        typeof(string)
    };

    /// <summary>
    /// Collection of mappers to be used for object->node mapping. The order of mappers execution is inverted, i.e. mapper added as last one will be used as first one, which allows to register the most specific mappers as first ones.
    /// </summary>
    public Stack<NodeMapper> Mappers { get; } = new(new NodeMapper[]
    {
        PocoMapper.Instance,
        EnumerableMapper.Instance,
        ExpandoObjectMapper.Instance,
        FormattableValueMapper.Instance,
        JsonElementObjectMapper.Instance,
        JsonElementArrayMapper.Instance,
        JsonElementValueMapper.Instance,
        ExpectationValueMapper.Instance,
    });
}