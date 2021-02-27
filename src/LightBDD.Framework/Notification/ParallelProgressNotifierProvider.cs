using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification.Implementation;

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// Class providing implementations of <see cref="IFeatureProgressNotifier"/> and <see cref="IScenarioProgressNotifier"/> interfaces.
    /// The implementations are designed to provide notifications that would be still understandable when scenarios are executed in parallel.
    /// Each notification entry is prefixed with text showing total number of processed, failed and currently running scenarios as well as scenario number the message is notified for.
    /// </summary>
    public class ParallelProgressNotifierProvider
    {
        private readonly ProgressManager _manager = new ProgressManager();
        /// <summary>
        /// Returns default instance of provider.
        /// </summary>
        public static ParallelProgressNotifierProvider Default { get; } = new ParallelProgressNotifierProvider();

        /// <summary>
        /// Default constructor.
        /// The class should not be instantiated directly, but rather <see cref="Default"/> property should be used to get provider instance.
        /// </summary>
        protected ParallelProgressNotifierProvider()
        {
        }

        /// <summary>
        /// Creates <see cref="IFeatureProgressNotifier"/> instance which would call <paramref name="onNotify"/> actions with formatted notifications.
        /// The created notifier does nothing with formatted notifications that is why <paramref name="onNotify"/> should contain at least 1 action in order to get notifications working.
        /// </summary>
        /// <param name="onNotify">Actions that would be called with formatted notifications.</param>
        /// <returns><see cref="IFeatureProgressNotifier"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="onNotify"/> is <c>null</c>.</exception>
        [Obsolete]
        public IFeatureProgressNotifier CreateFeatureProgressNotifier(params Action<string>[] onNotify)
        {
            if (onNotify == null)
                throw new ArgumentNullException(nameof(onNotify));
            return new ParallelProgressNotifier(_manager, onNotify);
        }

        /// <summary>
        /// Creates <see cref="IScenarioProgressNotifier"/> instance which would call <paramref name="onNotify"/> actions with formatted notifications.
        /// The created notifier does nothing with formatted notifications that is why <paramref name="onNotify"/> should contain at least 1 action in order to get notifications working.
        /// </summary>
        /// <param name="onNotify">Actions that would be called with formatted notifications.</param>
        /// <returns><see cref="IScenarioProgressNotifier"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="onNotify"/> is <c>null</c>.</exception>
        [Obsolete]
        public IScenarioProgressNotifier CreateScenarioProgressNotifier(params Action<string>[] onNotify)
        {
            if (onNotify == null)
                throw new ArgumentNullException(nameof(onNotify));
            return new ParallelProgressNotifier(_manager, onNotify);
        }

        /// <summary>
        /// Creates <see cref="IProgressNotifier"/> instance which would call <paramref name="onNotify"/> actions with formatted notifications.
        /// The created notifier does nothing with formatted notifications that is why <paramref name="onNotify"/> should contain at least 1 action in order to get notifications working.
        /// </summary>
        /// <param name="onNotify">Actions that would be called with formatted notifications.</param>
        /// <returns><see cref="IProgressNotifier"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="onNotify"/> is <c>null</c>.</exception>
        public IProgressNotifier CreateProgressNotifier(params Action<string>[] onNotify)
        {
            if (onNotify == null)
                throw new ArgumentNullException(nameof(onNotify));
            return new ParallelProgressNotifier(_manager, onNotify);
        }
    }
}