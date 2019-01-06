using System;

namespace LightBDD.Core.Dependencies
{
    /// <summary>
    /// An abstract class allowing to configure container.
    /// </summary>
    public abstract class ContainerConfigurator
    {
        /// <summary>
        /// Registers the <paramref name="instance"/> in the container.
        /// If provided instance implements <see cref="IDisposable"/> interface, it will be disposed with container disposal, unless <see cref="RegistrationOptions.IsExternallyOwned"/> is set to true in <paramref name="options"/> parameter.
        ///
        /// The <paramref name="options"/> parameter allows to configure registration of the <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">Instance to register.</param>
        /// <param name="options">Registration options.</param>
        public abstract void RegisterInstance(object instance, RegistrationOptions options);
    }
}