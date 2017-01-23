using System.Diagnostics;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Notification
{
    [DebuggerStepThrough]
    public class DelegatingScenarioProgressNotifier : IScenarioProgressNotifier
    {
        private readonly IScenarioProgressNotifier[] _notifiers;

        public DelegatingScenarioProgressNotifier(params IScenarioProgressNotifier[] notifiers)
        {
            _notifiers = notifiers;
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