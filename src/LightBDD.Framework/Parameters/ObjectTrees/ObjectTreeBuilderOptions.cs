#nullable enable
using System;
using System.Collections.Generic;
using LightBDD.Framework.Parameters.ObjectTrees.Providers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeBuilderOptions
{
    public HashSet<Type> ValueTypes { get; } = new()
    {
        typeof(string)
    };

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