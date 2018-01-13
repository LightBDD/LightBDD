using System;
using System.Linq;
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
        /// Replaces the <see cref="Notifier"/> with <paramref name="notifier"/> value.
        /// </summary>
        /// <param name="notifier">New notifier to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifier"/> is null.</exception>
        [Obsolete("Use " + nameof(SetNotifier) + " instead that better reflect the effect")]
        public FeatureProgressNotifierConfiguration UpdateNotifier(IFeatureProgressNotifier notifier)
        {
            return SetNotifier(notifier);
        }

        /// <summary>
        /// Replaces the <see cref="Notifier"/> with <paramref name="notifier"/> value.
        /// </summary>
        /// <param name="notifier">New notifier to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifier"/> is null.</exception>
        public FeatureProgressNotifierConfiguration SetNotifier(IFeatureProgressNotifier notifier)
        {
            ThrowIfSealed();
            Notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            return this;
        }

        /// <summary>
        /// Appends <paramref name="notifiers"/> to existing <see cref="Notifier"/> making all of them used during notification.
        /// </summary>
        /// <param name="notifiers">Notifiers to append</param>
        /// <returns>Self</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> collection or any of it's item is null.</exception>
        public FeatureProgressNotifierConfiguration AppendNotifiers(params IFeatureProgressNotifier[] notifiers)
        {
            ThrowIfSealed();
            if (notifiers == null)
                throw new ArgumentNullException(nameof(notifiers));
            Notifier = DelegatingFeatureProgressNotifier.Compose(Enumerable.Repeat(Notifier, 1).Concat(notifiers));
            return this;
        }

        /// <summary>
        /// Sets <see cref="Notifier"/> to <see cref="NoProgressNotifier.Default"/> instance that does not report any notifications.
        /// </summary>
        /// <returns>Self.</returns>
        public FeatureProgressNotifierConfiguration ClearNotifier() => SetNotifier(NoProgressNotifier.Default);
    }
}