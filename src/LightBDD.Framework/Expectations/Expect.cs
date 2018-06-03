using System.Diagnostics;
using LightBDD.Framework.Expectations.Implementation;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Type allowing to define expectations.
    /// </summary>
    [DebuggerStepThrough]
    public static class Expect
    {
        /// <summary>
        /// Creates expectation composer.
        /// </summary>
        public static IExpectationComposer To => new ExpectationComposer();

        /// <summary>
        /// Returns <see cref="TypeRef{T}"/> that is used for inferring type <typeparamref name="T"/> in generic methods.
        /// </summary>
        public static TypeRef<T> Type<T>()
        {
            return new TypeRef<T>();
        }
    }
}