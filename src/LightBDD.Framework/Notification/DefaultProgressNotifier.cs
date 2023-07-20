using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Reporting.Formatters;
#pragma warning disable 618

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// The default implementation of <see cref="IProgressNotifier"/> which renders the notification text and delegates to provided notification actions configured in constructor.
    /// </summary>
    public class DefaultProgressNotifier : IProgressNotifier
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
                    NotifyStepFileAttached(stepFileAttached.Step, stepFileAttached.Attachment);
                    break;
                case TestRunStarting testRunStarting:
                    NotifyTestRunStart(testRunStarting.TestRun);
                    break;
                case TestRunFinished testRunFinished:
                    NotifyTestRunFinished(testRunFinished.Result);
                    break;
            }
        }

        private void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _onNotify($"SCENARIO: {FormatLabels(scenario.Labels)}{scenario.Name}");
        }

        private void NotifyScenarioFinished(IScenarioResult scenario)
        {
            var scenarioText = scenario.ExecutionTime != null
                ? $"  SCENARIO RESULT: {scenario.Status} after {scenario.ExecutionTime.Duration.FormatPretty()}"
                : $"  SCENARIO RESULT: {scenario.Status}";

            var scenarioDetails = !string.IsNullOrWhiteSpace(scenario.StatusDetails)
                ? $"{Environment.NewLine}    {scenario.StatusDetails.Replace(Environment.NewLine, Environment.NewLine + "    ")}"
                : string.Empty;

            _onNotify(scenarioText + scenarioDetails);
        }

        private void NotifyStepStart(IStepInfo step)
        {
            _onNotify($"  STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}: {step.Name}...");
        }

        private void NotifyStepFinished(IStepResult step)
        {
            var report = new List<string>
            {
                $"  STEP {step.Info.GroupPrefix}{step.Info.Number}/{step.Info.GroupPrefix}{step.Info.Total}: {step.Info.Name} ({step.Status} after {step.ExecutionTime.Duration.FormatPretty()})"
            };
            foreach (var parameter in step.Parameters)
            {
                switch (parameter.Details)
                {
                    case ITabularParameterDetails table:
                        report.Add($"    {parameter.Name}:");
                        report.Add(new TextTableRenderer(table).Render("    "));
                        break;
                    case ITreeParameterDetails tree:
                        report.Add($"    {parameter.Name}:");
                        report.Add(TextTreeRenderer.Render("    ", tree));
                        break;
                }
            }
            _onNotify(string.Join(Environment.NewLine, report));
        }

        private void NotifyStepComment(IStepInfo step, string comment)
        {
            _onNotify($"  STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}: /* {comment} */");
        }

        private void NotifyFeatureStart(IFeatureInfo feature)
        {
            _onNotify($"FEATURE: {FormatLabels(feature.Labels)}{feature.Name}{FormatDescription(feature.Description)}");
        }

        private void NotifyFeatureFinished(IFeatureResult feature)
        {
            _onNotify($"FEATURE FINISHED: {feature.Info.Name}");
        }

        private void NotifyTestRunFinished(ITestRunResult result)
        {
            _onNotify($"TEST RUN FINISHED: {result.Info.Name} ({result.OverallStatus} after {result.ExecutionTime.Duration.FormatPretty()})");
        }

        private void NotifyTestRunStart(ITestRunInfo testRun)
        {
            _onNotify($"TEST RUN STARTING: {testRun.Name}");
        }

        private void NotifyStepFileAttached(IStepInfo step, FileAttachment attachment)
        {
            _onNotify($"  STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}: 🔗{attachment.Name}: {attachment.FilePath}");
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