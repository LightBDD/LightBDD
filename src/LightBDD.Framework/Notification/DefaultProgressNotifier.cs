using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Reporting.Formatters;
#pragma warning disable 618

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// The default implementation of <see cref="IScenarioProgressNotifier"/> and <see cref="IFeatureProgressNotifier"/> which renders the notification text and delegates to provided notification actions configured in constructor.
    /// </summary>
    public class DefaultProgressNotifier : IProgressNotifier, IScenarioProgressNotifier, IFeatureProgressNotifier
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
                ? $"{Environment.NewLine}    {scenario.StatusDetails.Replace(Environment.NewLine, Environment.NewLine + "    ")}"
                : string.Empty;

            _onNotify(scenarioText + scenarioDetails);
        }

        /// <summary>
        /// Notifies that step has started.
        /// </summary>
        /// <param name="step">Step info.</param>
        public void NotifyStepStart(IStepInfo step)
        {
            _onNotify($"  STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}: {step.Name}...");
        }

        /// <summary>
        /// Notifies that step has finished.
        /// </summary>
        /// <param name="step">Step result.</param>
        public void NotifyStepFinished(IStepResult step)
        {
            var report = new List<string>
            {
                $"  STEP {step.Info.GroupPrefix}{step.Info.Number}/{step.Info.GroupPrefix}{step.Info.Total}: {step.Info.Name} ({step.Status} after {step.ExecutionTime.Duration.FormatPretty()})"
            };
            foreach (var parameter in step.Parameters)
            {
                if (parameter.Details is ITabularParameterDetails table)
                {
                    report.Add($"    {parameter.Name}:");
                    report.Add(new TextTableRenderer(table).Render("    "));
                }
            }
            _onNotify(string.Join(Environment.NewLine, report));
        }

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="step">Step info.</param>
        /// <param name="comment">Comment.</param>
        public void NotifyStepComment(IStepInfo step, string comment)
        {
            _onNotify($"  STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}: /* {comment} */");
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

        /// <inheritdoc />
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
                case StepFileAttached stepFileAttached:
                    NotifyStepAttached(stepFileAttached.Step, stepFileAttached.Attachment);
                    break;
            }
        }

        private void NotifyStepAttached(IStepInfo step, FileAttachment attachment)
        {
            _onNotify($"  STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}: File Attached - {attachment.Name}: {attachment.FilePath}");
        }

        private static string FormatDescription(string description)
        {
            return string.IsNullOrWhiteSpace(description)
                ? string.Empty
                : $"{Environment.NewLine}  {description.Replace(Environment.NewLine, Environment.NewLine + "  ")}";
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