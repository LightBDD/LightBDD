using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Fallback resolve behavior for the types that are not explicitly registered in the container.
    /// </summary>
    public enum FallbackResolveBehavior
    {
        /// <summary>
        /// Type is resolved as transient instance using it's public constructor.
        /// </summary>
        ResolveTransient,
        /// <summary>
        /// An <seealso cref="InvalidOperationException"/> exception is thrown, enforcing explicit registrations of all dependency types.
        /// </summary>
        ThrowException
    }
}