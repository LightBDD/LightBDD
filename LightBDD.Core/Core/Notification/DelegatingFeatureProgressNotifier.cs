using System.Diagnostics;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
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