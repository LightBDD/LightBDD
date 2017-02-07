using System;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification.Implementation
{
    [DebuggerStepThrough]
    internal class ParallelProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier
    {
        private readonly ProgressManager _manager;
        private int? _currentScenarioNumber;
        private readonly DefaultProgressNotifier _notifier;
        private readonly Action<string> _onNotify;

        public ParallelProgressNotifier(ProgressManager manager, Action<string>[] onNotify)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            if (onNotify == null || !onNotify.Any())
                throw new ArgumentException("At least one on notify action required", nameof(onNotify));

            _manager = manager;
            _onNotify = onNotify.Aggregate((current, next) => current + next);
            _notifier = new DefaultProgressNotifier(NotifyProgress);
        }

        private void NotifyProgress(string message)
        {
            var progress = _manager.GetProgress();
            var header = $"Fi={progress.FinishedScenarios:D3},Fa={progress.FailedScenarios:D3},Pe={progress.PendingScenarios:D3} #{_currentScenarioNumber,3}> ";
            _onNotify(header + message.Replace("\n", "\n" + new string(' ', header.Length)));
        }

        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            _notifier.NotifyFeatureStart(feature);
        }

        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            _notifier.NotifyFeatureFinished(feature);
        }

        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _currentScenarioNumber = _manager.StartNewScenario();
            _notifier.NotifyScenarioStart(scenario);
        }

        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            _manager.CaptureScenarioResult(scenario.Status);
            _notifier.NotifyScenarioFinished(scenario);
            _manager.FinishScenario();
        }

        public void NotifyStepStart(IStepInfo step)
        {
            _notifier.NotifyStepStart(step);
        }

        public void NotifyStepFinished(IStepResult step)
        {
            _notifier.NotifyStepFinished(step);
        }

        public void NotifyStepComment(IStepInfo step, string comment)
        {
            _notifier.NotifyStepComment(step, comment);
        }
    }
}