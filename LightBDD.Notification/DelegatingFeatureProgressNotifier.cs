using System.Diagnostics;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Notification
{
    [DebuggerStepThrough]
    public class DelegatingFeatureProgressNotifier : IFeatureProgressNotifier
    {
        private readonly IFeatureProgressNotifier[] _notifiers;

        public DelegatingFeatureProgressNotifier(params IFeatureProgressNotifier[] notifiers)
        {
            _notifiers = notifiers;
        }

        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyFeatureStart(feature);
        }

        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyFeatureFinished(feature);
        }
    }
}