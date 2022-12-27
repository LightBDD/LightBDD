using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public abstract class ArrayMapper : NodeMapper
{
    protected ArrayMapper() : base(ObjectTreeNodeKind.Array) { }
    /// <summary>
    /// Tries to interpret provided object <paramref name="o"/> as collection and returns its content or <c>null</c> if given object is not supported.
    /// </summary>
    public abstract IEnumerable<object?> GetItems(object o);
}