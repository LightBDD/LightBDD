using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class EnumerableArrayProvider : IArrayProvider
{
    public static readonly EnumerableArrayProvider Instance = new();

    private EnumerableArrayProvider() { }

    public IEnumerable<object>? TryProvide(object o)
    {
        if (o is IEnumerable e)
            return e.Cast<object>();
        return null;
    }
}