#nullable enable
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public abstract class ObjectMapper : NodeMapper
{
    protected ObjectMapper() : base(ObjectTreeNodeKind.Object) { }

    /// <summary>
    /// Tries to interpret provided object <paramref name="o"/> as complex object and returns its properties or <c>null</c> if given object is not supported.
    /// </summary>
    public abstract IEnumerable<KeyValuePair<string, object?>> GetProperties(object o);
}