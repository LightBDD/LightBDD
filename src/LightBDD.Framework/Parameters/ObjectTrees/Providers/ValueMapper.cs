#nullable enable
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public abstract class ValueMapper : NodeMapper
{
    protected ValueMapper() : base(ObjectTreeNodeKind.Value) { }

    /// <summary>
    /// Tries to interpret provided object <paramref name="o"/> as value object and return.
    /// </summary>
    public abstract object? GetValue(object o);
}