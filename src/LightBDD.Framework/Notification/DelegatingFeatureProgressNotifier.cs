using System;
using System.Diagnostics;
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
        private readonly IFeatureProgressNotifier[] _notifiers;
        /// <summary>
        /// Constructor configuring notifier to delegate all the notifications to provided <paramref name="notifiers"/>.
        /// </summary>
        /// <param name="notifiers">Notifiers to delegate notifications to.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="notifiers"/> is null.</exception>
        public DelegatingFeatureProgressNotifier(params IFeatureProgressNotifier[] notifiers)
        {
            if (notifiers == null)
                throw new ArgumentNullException(nameof(notifiers));
            _notifiers = notifiers;
        }
        /// <summary>
        /// Notifies that feature has started.
        /// </summary>
        /// <param name="feature">Feature info.</param>
        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyFeatureStart(feature);
        }
        /// <summary>
        /// Notifies that feature has finished.
        /// </summary>
        /// <param name="feature">Feature result.</param>
        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyFeatureFinished(feature);
        }
    }
}