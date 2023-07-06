using System;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
#pragma warning disable 618

namespace LightBDD.Framework.Notification.Implementation
{
    internal class ParallelProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier, IProgressNotifier
    {
        private readonly ProgressManager _manager;
        private readonly DefaultProgressNotifier _notifier;
        private readonly Action<string> _onNotify;

        public ParallelProgressNotifier(ProgressManager manager, Action<string>[] onNotify)
        {
            if (onNotify == null || !onNotify.Any())
                throw new ArgumentException("At least one on notify action required", nameof(onNotify));

            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _onNotify = onNotify.Aggregate((current, next) => current + next);
            _notifier = new DefaultProgressNotifier(NotifyProgress);
        }

        private void NotifyProgress(string message)
        {
            var progress = _manager.GetProgress();
            var header = $"Fi={progress.FinishedScenarios:D3},Fa={progress.FailedScenarios:D3},Pe={progress.PendingScenarios:D3} #{progress.CurrentScenarioNumber,3}> ";
            _onNotify(header + message.Replace(Environment.NewLine, Environment.NewLine + new string(' ', header.Length)));
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
            _manager.StartNewScenario();
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

        public void Notify(ProgressEvent e)
        {
            switch (e)
            {
                case FeatureFinished featureFinished:
                    NotifyFeatureFinished(featureFinished.Result);
                    break;
                case FeatureStarting featureStarting:
                    NotifyFeatureStart(featureStarting.Feature);
                    break;
                case ScenarioFinished scenarioFinished:
                    NotifyScenarioFinished(scenarioFinished.Result);
                    break;
                case ScenarioStarting scenarioStarting:
                    NotifyScenarioStart(scenarioStarting.Scenario);
                    break;
                case StepCommented stepCommented:
                    NotifyStepComment(stepCommented.Step, stepCommented.Comment);
                    break;
                case StepFinished stepFinished:
                    NotifyStepFinished(stepFinished.Result);
                    break;
                case StepStarting stepStarting:
                    NotifyStepStart(stepStarting.Step);
                    break;
                default:
                    _notifier.Notify(e);
                    break;
            }
        }
    }
}