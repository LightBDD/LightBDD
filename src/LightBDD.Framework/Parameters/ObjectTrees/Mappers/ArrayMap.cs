#nullable enable
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Mappers;

/// <summary>
/// Array map produced by <seealso cref="ArrayMapper"/>
/// </summary>
public class ArrayMap
{
    /// <summary>
    /// Array items
    /// </summary>
    public IEnumerable<object?> Items { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ArrayMap(IEnumerable<object?> items)
    {
        Items = items;
    }
}