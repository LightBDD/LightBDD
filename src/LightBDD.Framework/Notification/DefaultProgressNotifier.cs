using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// The default implementation of <see cref="IScenarioProgressNotifier"/> and <see cref="IFeatureProgressNotifier"/> which renders the notification text and delegates to provided notification actions configured in constructor.
    /// </summary>
    [DebuggerStepThrough]
    public class DefaultProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier
    {
        private readonly Action<string> _onNotify;

        /// <summary>
        /// Initializes the notifier with <paramref name="onNotify"/> actions that will be used to delegate the rendered notification text.
        /// </summary>
        /// <param name="onNotify"></param>
        public DefaultProgressNotifier(params Action<string>[] onNotify)
        {
            _onNotify = onNotify.Aggregate((current, next) => current + next);
        }

        /// <summary>
        /// Notifies that scenario has started.
        /// </summary>
        /// <param name="scenario">Scenario info.</param>
        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _onNotify($"SCENARIO: {FormatLabels(scenario.Labels)}{scenario.Name}");
        }

        /// <summary>
        /// Notifies that scenario has finished.
        /// </summary>
        /// <param name="scenario">Scenario result.</param>
        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            var scenarioText = scenario.ExecutionTime != null
                ? $"  SCENARIO RESULT: {scenario.Status} after {scenario.ExecutionTime.Duration.FormatPretty()}"
                : $"  SCENARIO RESULT: {scenario.Status}";

            var scenarioDetails = !string.IsNullOrWhiteSpace(scenario.StatusDetails)
                ? $"\n    {scenario.StatusDetails.Replace("\n", "\n    ")}"
                : string.Empty;

            _onNotify(scenarioText + scenarioDetails);
        }

        /// <summary>
        /// Notifies that step has started.
        /// </summary>
        /// <param name="step">Step info.</param>
        public void NotifyStepStart(IStepInfo step)
        {
            _onNotify($"  STEP {step.Number}/{step.Total}: {step.Name}...");
        }

        /// <summary>
        /// Notifies that step has finished.
        /// </summary>
        /// <param name="step">Step result.</param>
        public void NotifyStepFinished(IStepResult step)
        {
            _onNotify($"  STEP {step.Info.Number}/{step.Info.Total}: {step.Info.Name} ({step.Status} after {step.ExecutionTime.Duration.FormatPretty()})");
        }

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="step">Step info.</param>
        /// <param name="comment">Comment.</param>
        public void NotifyStepComment(IStepInfo step, string comment)
        {
            _onNotify($"  STEP {step.Number}/{step.Total}: /* {comment} */");
        }

        /// <summary>
        /// Notifies that feature has started.
        /// </summary>
        /// <param name="feature">Feature info.</param>
        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            _onNotify($"FEATURE: {FormatLabels(feature.Labels)}{feature.Name}{FormatDescription(feature.Description)}");
        }

        /// <summary>
        /// Notifies that feature has finished.
        /// </summary>
        /// <param name="feature">Feature result.</param>
        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            _onNotify($"FEATURE FINISHED: {feature.Info.Name}");
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
    }
}