#nullable enable
using System;
using System.Linq;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Implementation;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize scenario progress notification behavior.
    /// </summary>
    public class ProgressNotifierConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Returns progress notifier.<br/>
        /// By default it is configured to not report any notifications.
        /// </summary>
        public IProgressNotifier Notifier { get; private set; } = NoProgressNotifier.Default;

        /// <summary>
        /// Appends <paramref name="notifiers"/> to existing <see cref="Notifier"/> making all of them used during notification.
        /// </summary>
        /// <param name="notifiers">Notifiers to append</param>
        /// <returns>Self</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> collection or any of it's item is null.</exception>
        public ProgressNotifierConfiguration Append(params IProgressNotifier[] notifiers)
        {
            ThrowIfSealed();
            if (notifiers == null)
                throw new ArgumentNullException(nameof(notifiers));
            Notifier = DelegatingProgressNotifier.Compose(Enumerable.Repeat(Notifier, 1).Concat(notifiers));
            return this;
        }

        /// <summary>
        /// Prepends <paramref name="notifiers"/> to existing <see cref="Notifier"/> making all of them used during notification.
        /// </summary>
        /// <param name="notifiers">Notifiers to append</param>
        /// <returns>Self</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> collection or any of it's item is null.</exception>
        public ProgressNotifierConfiguration Prepend(params IProgressNotifier[] notifiers)
        {
            ThrowIfSealed();
            if (notifiers == null)
                throw new ArgumentNullException(nameof(notifiers));
            Notifier = DelegatingProgressNotifier.Compose(notifiers.Concat(Enumerable.Repeat(Notifier, 1)));
            return this;
        }

        /// <summary>
        /// Clears <see cref="Notifier"/> to use instance that does not report any notifications.
        /// </summary>
        /// <returns>Self.</returns>
        public ProgressNotifierConfiguration Clear()
        {
            ThrowIfSealed();
            Notifier = NoProgressNotifier.Default;
            return this;
        }
    }
}