using System.Diagnostics;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification
{
    [DebuggerStepThrough]
    public class DelegatingProgressNotifier : IProgressNotifier
    {
        private readonly IProgressNotifier[] _notifiers;

        public DelegatingProgressNotifier(params IProgressNotifier[] notifiers)
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

        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyScenarioStart(scenario);
        }

        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyScenarioFinished(scenario);
        }

        public void NotifyStepStart(IStepInfo step)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyStepStart(step);
        }

        public void NotifyStepFinished(IStepResult step)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyStepFinished(step);
        }

        public void NotifyStepComment(IStepInfo step, string comment)
        {
            foreach (var notifier in _notifiers)
                notifier.NotifyStepComment(step, comment);
        }
    }
}