using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public class EnumerableMapper : ArrayMapper
{
    public static readonly EnumerableMapper Instance = new();
    private EnumerableMapper() { }

    public override bool CanMap(object obj) => obj is IEnumerable;
    public override IEnumerable<object> GetItems(object o) => ((IEnumerable)o).Cast<object>();
}