#nullable enable
using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public interface IObjectProvider
{
    /// <summary>
    /// Tries to interpret provided object <paramref name="o"/> as complex object and returns its properties or <c>null</c> if given object is not supported.
    /// </summary>
    IEnumerable<KeyValuePair<string, object?>>? TryProvide(object o);
}