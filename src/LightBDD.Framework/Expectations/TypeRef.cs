using System.Diagnostics;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Helper type allowing to infer type <typeparamref name="T"/> in generic methods.
    /// </summary>
    [DebuggerStepThrough]
    public struct TypeRef<T>{}
}