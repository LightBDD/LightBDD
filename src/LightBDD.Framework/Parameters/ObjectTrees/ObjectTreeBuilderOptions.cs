#nullable enable
using System;
using System.Collections.Generic;
using LightBDD.Framework.Parameters.ObjectTrees.Providers;

namespace LightBDD.Framework.Parameters.ObjectTrees;

public class ObjectTreeBuilderOptions
{
    public HashSet<Type> ValueTypes { get; } = new()
    {
        typeof(string),
        typeof(IFormattable)
    };

    public Stack<IArrayProvider> ArrayProviders { get; } = new(new[] { EnumerableArrayProvider.Instance });
    public Stack<IObjectProvider> ObjectProviders { get; } = new(new[] { PocoObjectProvider.Instance });
}