using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// Feature progress notifier allowing to delegate notification to zero or more notifiers.
    /// </summary>
    [DebuggerStepThrough]
    public class DelegatingFeatureProgressNotifier : IFeatureProgressNotifier
    {
        /// <summary>
        /// Returns notifiers used for notifications.
        /// </summary>
        public IEnumerable<IFeatureProgressNotifier> Notifiers { get; }

        /// <summary>
        /// Constructor configuring notifier to delegate all the notifications to provided <paramref name="notifiers"/>.
        /// </summary>
        /// <param name="notifiers">Notifiers to delegate notifications to.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> is null.</exception>
        public DelegatingFeatureProgressNotifier(params IFeatureProgressNotifier[] notifiers)
        : this(EnsureNotNull(notifiers))
        {
        }

        private DelegatingFeatureProgressNotifier(IEnumerable<IFeatureProgressNotifier> notifiers)
        {
            Notifiers = notifiers.ToArray();
        }

        /// <summary>
        /// Notifies that feature has started.
        /// </summary>
        /// <param name="feature">Feature info.</param>
        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyFeatureStart(feature);
        }
        /// <summary>
        /// Notifies that feature has finished.
        /// </summary>
        /// <param name="feature">Feature result.</param>
        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            foreach (var notifier in Notifiers)
                notifier.NotifyFeatureFinished(feature);
        }

        /// <summary>
        /// Composes <see cref="IFeatureProgressNotifier"/> from provided notifiers where
        /// any notifiers of <see cref="NoProgressNotifier"/> will be excluded
        /// and any notifiers of <see cref="DelegatingFeatureProgressNotifier"/> will be flattened.
        /// </summary>
        /// <param name="notifiers">Notifiers to compose.</param>
        /// <returns>Composition of provided notifiers.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="notifiers"/> or any of it's items is null.</exception>
        public static IFeatureProgressNotifier Compose(IEnumerable<IFeatureProgressNotifier> notifiers)
        {
            var results = new List<IFeatureProgressNotifier>();

            FlattenNotifiers(results, EnsureNotNull(notifiers));

            if (!results.Any())
                return NoProgressNotifier.Default;
            return results.Count == 1
                ? results[0]
                : new DelegatingFeatureProgressNotifier(results);
        }

        private static void FlattenNotifiers(ICollection<IFeatureProgressNotifier> output, IEnumerable<IFeatureProgressNotifier> notifiers)
        {
            foreach (var notifier in notifiers)
            {
                if (notifier is NoProgressNotifier)
                    continue;
                if (notifier is DelegatingFeatureProgressNotifier delegating)
                    FlattenNotifiers(output, delegating.Notifiers);
                else
                    output.Add(notifier);
            }
        }

        private static IEnumerable<IFeatureProgressNotifier> EnsureNotNull(IEnumerable<IFeatureProgressNotifier> notifiers)
        {
            if (notifiers == null)
                throw new ArgumentNullException(nameof(notifiers));

            return notifiers.Select((notifier, i) => notifier ?? throw new ArgumentNullException($"{nameof(notifiers)}[{i}]"));
        }
    }
}