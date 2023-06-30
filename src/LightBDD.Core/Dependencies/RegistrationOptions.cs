using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// Dependency registration options.
    /// </summary>
    public class RegistrationOptions
    {
        private readonly HashSet<Type> _asTypes = new();
        /// <summary>
        /// Returns true if configured dependency is externally owned or false if container controls it's lifetime.
        /// </summary>
        public bool IsExternallyOwned { get; private set; }
        /// <summary>
        /// Returns list of types for whose configured dependency should be returned during resolution.
        /// </summary>
        public IEnumerable<Type> AsTypes => _asTypes;

        /// <summary>
        /// Returns true if <see cref="AsTypes"/> is empty and the configured dependency should be resolvable by it's implementation type.
        /// </summary>
        public bool IsRegisteredAsSelf => !AsTypes.Any();

        /// <summary>
        /// Specifies that configured dependency is maintained externally to the container, so that it should not be disposed during container disposal.
        /// </summary>
        public RegistrationOptions ExternallyOwned()
        {
            IsExternallyOwned = true;
            return this;
        }

        /// <summary>
        /// Adds types specified by <paramref name="types"/> parameter to the list of types for whose the configured dependency should be returned during resolution.
        /// </summary>
        public RegistrationOptions As(params Type[] types)
        {
            foreach (var type in types)
                _asTypes.Add(type);
            return this;
        }

        /// <summary>
        /// Adds <typeparamref name="T"/> to the list of types for whose the configured dependency should be returned during resolution.
        /// </summary>
        public RegistrationOptions As<T>() => As(typeof(T));
    }
}