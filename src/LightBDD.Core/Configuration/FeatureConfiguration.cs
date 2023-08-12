using System;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// A base class of FeatureConfiguration with sealed state detection.
    /// </summary>
    public abstract class FeatureConfiguration
    {
        internal void Seal()
        {
            IsSealed = true;
        }

        /// <summary>
        /// Returns true if configuration is sealed.
        /// </summary>
        protected bool IsSealed { get; private set; }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> when configuration is already sealed.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when configuration is sealed.</exception>
        protected void ThrowIfSealed()
        {
            if (IsSealed)
                throw new InvalidOperationException("Feature configuration is sealed. Please update configuration only during LightBDD initialization.");
        }
    }
}