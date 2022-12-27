#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using LightBDD.Framework.Parameters.ObjectTrees.Providers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeBuilderOptions
{
    public HashSet<Type> ValueTypes { get; } = new()
    {
        typeof(string),
        typeof(IFormattable)
    };

    public Stack<NodeMapper> Mappers { get; } = new(new NodeMapper[]
    {
        PocoMapper.Instance,
        EnumerableMapper.Instance,
        ExpandoMapper.Instance
    });
}