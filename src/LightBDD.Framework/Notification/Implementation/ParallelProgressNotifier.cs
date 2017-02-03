using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification.Implementation
{
    [DebuggerStepThrough]
    internal class ParallelProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier
    {
        private readonly ProgressManager _manager;
        private readonly Action<string> _onNotify;
        private int? _currentScenarioNumber;

        public ParallelProgressNotifier(ProgressManager manager, Action<string>[] onNotify)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));
            if (onNotify == null || !onNotify.Any())
                throw new ArgumentException("At least one on notify action required", nameof(onNotify));

            _manager = manager;
            _onNotify = onNotify.Aggregate((current, next) => current + next);
        }

        private void NotifyProgress(string message)
        {
            var progress = _manager.GetProgress();
            var header = $"Fi={progress.FinishedScenarios:D3},Fa={progress.FailedScenarios:D3},Pe={progress.PendingScenarios:D3} #{_currentScenarioNumber,3}> ";

            _onNotify(header + message.Replace("\n", "\n" + new string(' ', header.Length)));
        }

        private static string FormatDescription(string description)
        {
            return string.IsNullOrWhiteSpace(description)
                ? string.Empty
                : $"\n  {description.Replace("\n", "\n  ")}";
        }

        private static string FormatLabels(IEnumerable<string> labels)
        {
            var joinedLabels = string.Join("][", labels);
            if (joinedLabels != string.Empty)
                joinedLabels = "[" + joinedLabels + "] ";
            return joinedLabels;
        }

        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            NotifyProgress($"FEATURE: {FormatLabels(feature.Labels)}{feature.Name}{FormatDescription(feature.Description)}");
        }

        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            NotifyProgress($"FEATURE FINISHED: {feature.Info.Name}");
        }

        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _currentScenarioNumber = _manager.StartNewScenario();
            NotifyProgress($"SCENARIO: {FormatLabels(scenario.Labels)}{scenario.Name}");
        }

        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            _manager.CaptureScenarioResult(scenario.Status);
            var scenarioText = scenario.ExecutionTime != null
                ? $"  SCENARIO RESULT: {scenario.Status} after {scenario.ExecutionTime.Duration.FormatPretty()}"
                : $"  SCENARIO RESULT: {scenario.Status}";

            var scenarioDetails = !string.IsNullOrWhiteSpace(scenario.StatusDetails)
                ? $"\n    {scenario.StatusDetails.Replace("\n", "\n    ")}"
                : string.Empty;

            NotifyProgress(scenarioText + scenarioDetails);
            _manager.FinishScenario();
        }

        public void NotifyStepStart(IStepInfo step)
        {
            NotifyProgress($"  STEP {step.Number}/{step.Total}: {step.Name}...");
        }

        public void NotifyStepFinished(IStepResult step)
        {
            NotifyProgress($"  STEP {step.Info.Number}/{step.Info.Total}: {step.Info.Name} ({step.Status} after {step.ExecutionTime.Duration.FormatPretty()})");
        }

        public void NotifyStepComment(IStepInfo step, string comment)
        {
            NotifyProgress($"  STEP {step.Number}/{step.Total}: /* {comment} */");
        }
    }
}