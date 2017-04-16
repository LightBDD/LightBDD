using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Notification;

namespace LightBDD.Framework.Notification.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize feature progress notification behavior.
    /// </summary>
    public class FeatureProgressNotifierConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns feature progress notifier.
        /// By default it is initialized with <see cref="NoProgressNotifier.Default"/> instance.
        /// </summary>
        public IFeatureProgressNotifier Notifier { get; private set; } = NoProgressNotifier.Default;

        /// <summary>
        /// Updates <see cref="Notifier"/> with new value.
        /// </summary>
        /// <param name="notifier">New notifier to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifier"/> is null.</exception>
        public FeatureProgressNotifierConfiguration UpdateNotifier(IFeatureProgressNotifier notifier)
        {
            ThrowIfSealed();
            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));
            Notifier = notifier;
            return this;
        }
    }
}