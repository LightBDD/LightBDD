using System.Collections.Generic;

namespace LightBDD.Framework.Parameters.ObjectTrees.Providers;

public interface IArrayProvider
{
    /// <summary>
    /// Tries to interpret provided object <paramref name="o"/> as collection and returns its content or <c>null</c> if given object is not supported.
    /// </summary>
    IEnumerable<object?>? TryProvide(object o);
}