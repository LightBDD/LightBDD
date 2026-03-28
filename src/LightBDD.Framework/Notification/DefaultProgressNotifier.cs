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
using LightBDD.Framework.Reporting.Formatters.PlainText;

#pragma warning disable 618

namespace LightBDD.Framework.Notification
{
    /// <summary>
    /// The default implementation of <see cref="IScenarioProgressNotifier"/> and <see cref="IFeatureProgressNotifier"/> which renders the notification text and delegates to provided notification actions configured in constructor.
    /// <br/>
    /// This notifier is fully configurable through its properties. By default it produces compact flat output
    /// with step references as prefixes and no nesting-based indentation.
    /// <br/>
    /// To get hierarchical, YAML-like indented output, set <see cref="IndentLength"/> to a positive value
    /// and adjust the step word positioning properties, or use <see cref="SimpleIndentedProgressNotifier"/>
    /// which pre-configures these settings.
    /// </summary>
    public class DefaultProgressNotifier : IProgressNotifier, IScenarioProgressNotifier, IFeatureProgressNotifier
    {
        private readonly Action<string> _onNotify;

        /// <summary>
        /// When set to <c>false</c>, suppresses the finish notification for steps that passed successfully.
        /// A finish notification is still written for non-passing statuses (Failed, Ignored, Bypassed, etc.).
        /// Default is <c>true</c>.
        /// </summary>
        public bool WriteSuccessMessageForBasicSteps { get; set; } = true;

        /// <summary>
        /// When set to <c>true</c>, includes the total step count alongside the current step number.
        /// For example, "STEP 1.3/1.5" instead of "STEP 1.3".
        /// Default is <c>true</c>.
        /// </summary>
        public bool ShowFinalStepWithEachStep { get; set; } = true;

        /// <summary>
        /// The number of spaces used to indent each level of sub-step nesting.
        /// When set to 0 or less, a fixed two-space indent is used regardless of nesting depth.
        /// When set to a positive value, hierarchical indentation is applied based on nesting level.
        /// Default is 0 (flat indentation).
        /// </summary>
        public int IndentLength { get; set; }

        /// <summary>
        /// Controls how the step word and number are rendered in <b>step start</b> notifications.
        /// Default is <see cref="StepWordAndStepNumberBehaviour.IncludeAsPrefix"/>.
        /// </summary>
        public StepWordAndStepNumberBehaviour StepWordAndStepNumberOnStart { get; set; } = StepWordAndStepNumberBehaviour.IncludeAsPrefix;

        /// <summary>
        /// Controls how the step word and number are rendered in <b>step finish</b> notifications.
        /// Default is <see cref="StepWordAndStepNumberBehaviour.IncludeAsPrefix"/>.
        /// </summary>
        public StepWordAndStepNumberBehaviour StepWordAndStepNumberOnFinish { get; set; } = StepWordAndStepNumberBehaviour.IncludeAsPrefix;

        /// <summary>
        /// When set to <c>true</c>, includes the step name in the finish notification alongside the outcome.
        /// Default is <c>true</c>.
        /// </summary>
        public bool IncludeStepNameOnFinish { get; set; } = true;

        /// <summary>
        /// When set to <c>true</c>, appends "..." after the step name in start notifications.
        /// Default is <c>true</c>.
        /// </summary>
        public bool IncludeEllipsisAfterStep { get; set; } = true;

        /// <summary>
        /// When set to <c>true</c>, writes a blank line after the feature start notification.
        /// Default is <c>false</c>.
        /// </summary>
        public bool WriteBlankLineAfterFeatureStart { get; set; }

        /// <summary>
        /// When set to <c>true</c>, writes a blank line after the scenario start notification.
        /// Default is <c>false</c>.
        /// </summary>
        public bool WriteBlankLineAfterScenarioStart { get; set; }

        /// <summary>
        /// When set to <c>true</c>, writes blank lines before and after the scenario result notification.
        /// Default is <c>false</c>.
        /// </summary>
        public bool WriteBlankLinesAroundScenarioFinish { get; set; }

        /// <summary>
        /// Initializes the notifier with <paramref name="onNotify"/> actions that will be used to delegate the rendered notification text.
        /// </summary>
        /// <param name="onNotify"></param>
        public DefaultProgressNotifier(params Action<string>[] onNotify)
        {
            _onNotify = onNotify.Aggregate((current, next) => current + next);
        }

        /// <summary>
        /// Delegates the notification text to the configured notification actions.
        /// </summary>
        protected void OnNotify(string message) => _onNotify(message);

        /// <summary>
        /// Notifies that scenario has started.
        /// </summary>
        /// <param name="scenario">Scenario info.</param>
        public virtual void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _onNotify($"SCENARIO: {FormatLabels(scenario.Labels)}{scenario.Name}{FormatDescription(scenario.Description)}");
            if (WriteBlankLineAfterScenarioStart)
                _onNotify("");
        }

        /// <summary>
        /// Notifies that scenario has finished.
        /// </summary>
        /// <param name="scenario">Scenario result.</param>
        public virtual void NotifyScenarioFinished(IScenarioResult scenario)
        {
            if (WriteBlankLinesAroundScenarioFinish)
                _onNotify("");

            var scenarioIndent = IndentLength > 0 ? "" : "  ";
            var scenarioText = scenario.ExecutionTime != null
                ? $"{scenarioIndent}SCENARIO RESULT: {scenario.Status} after {scenario.ExecutionTime.Duration.FormatPretty()}"
                : $"{scenarioIndent}SCENARIO RESULT: {scenario.Status}";

            var scenarioDetails = !string.IsNullOrWhiteSpace(scenario.StatusDetails)
                ? $"{Environment.NewLine}    {scenario.StatusDetails.Replace(Environment.NewLine, Environment.NewLine + "    ")}"
                : string.Empty;

            _onNotify(scenarioText + scenarioDetails);

            if (WriteBlankLinesAroundScenarioFinish)
                _onNotify("");
        }

        /// <summary>
        /// Notifies that step has started.
        /// </summary>
        /// <param name="step">Step info.</param>
        public virtual void NotifyStepStart(IStepInfo step)
        {
            var indentPrefix = GetIndentPrefix(step);
            var notification = indentPrefix;
            var stepWordAndStepNumber = GetStepWordAndStepNumber(step);

            if (StepWordAndStepNumberOnStart == StepWordAndStepNumberBehaviour.IncludeAsPrefix)
                notification += stepWordAndStepNumber + ": ";

            notification += step.Name;

            if (IncludeEllipsisAfterStep)
                notification += "...";

            if (StepWordAndStepNumberOnStart == StepWordAndStepNumberBehaviour.IncludeAsSuffix)
                notification += $" ({stepWordAndStepNumber})";

            _onNotify(notification);
        }

        /// <summary>
        /// Notifies that step has finished.
        /// </summary>
        /// <param name="step">Step result.</param>
        public virtual void NotifyStepFinished(IStepResult step)
        {
            var indentPrefix = GetIndentPrefix(step.Info);
            var annotationIndent = GetAnnotationIndent(step.Info);
            var report = new List<string>();

            if (WriteSuccessMessageForBasicSteps || step.Status != ExecutionStatus.Passed)
            {
                var notification = indentPrefix;
                var stepWordAndStepNumber = GetStepWordAndStepNumber(step.Info);

                if (StepWordAndStepNumberOnFinish == StepWordAndStepNumberBehaviour.IncludeAsPrefix)
                    notification += stepWordAndStepNumber + (IncludeStepNameOnFinish ? ": " : ":");

                if (IncludeStepNameOnFinish)
                    notification += step.Info.Name;

                if (StepWordAndStepNumberOnFinish != StepWordAndStepNumberBehaviour.IncludeAsPrefix)
                    notification += "  =>";

                notification += $" ({step.Status} after {step.ExecutionTime.Duration.FormatPretty()})";

                if (StepWordAndStepNumberOnFinish == StepWordAndStepNumberBehaviour.IncludeAsSuffix)
                    notification += $" ({stepWordAndStepNumber})";

                report.Add(notification);
            }

            foreach (var parameter in step.Parameters)
            {
                switch (parameter.Details)
                {
                    case ITabularParameterDetails table:
                        report.Add($"{annotationIndent}{parameter.Name}:");
                        report.Add(new TextTableRenderer(table).Render(annotationIndent));
                        break;
                    case ITreeParameterDetails tree:
                        report.Add($"{annotationIndent}{parameter.Name}:");
                        report.Add(TextTreeRenderer.Render(annotationIndent, tree));
                        break;
                }
            }

            if (report.Count > 0)
                _onNotify(string.Join(Environment.NewLine, report));
        }

        /// <summary>
        /// Notifies that step has been commented.
        /// </summary>
        /// <param name="step">Step info.</param>
        /// <param name="comment">Comment.</param>
        public virtual void NotifyStepComment(IStepInfo step, string comment)
        {
            if (StepWordAndStepNumberOnStart == StepWordAndStepNumberBehaviour.IncludeAsPrefix)
                _onNotify($"{GetIndentPrefix(step)}{GetStepWordAndStepNumber(step)}: /* {comment} */");
            else
                _onNotify($"{GetAnnotationIndent(step)}=> /* {comment} */");
        }

        /// <summary>
        /// Notifies that feature has started.
        /// </summary>
        /// <param name="feature">Feature info.</param>
        public virtual void NotifyFeatureStart(IFeatureInfo feature)
        {
            var headerPad = IndentLength > 0 ? "  " : " ";
            _onNotify($"FEATURE:{headerPad}{FormatLabels(feature.Labels)}{feature.Name}{FormatDescription(feature.Description)}");
            if (WriteBlankLineAfterFeatureStart)
                _onNotify("");
        }

        /// <summary>
        /// Notifies that feature has finished.
        /// </summary>
        /// <param name="feature">Feature result.</param>
        public virtual void NotifyFeatureFinished(IFeatureResult feature)
        {
            _onNotify($"FEATURE FINISHED: {feature.Info.Name}");
        }

        /// <inheritdoc />
        public virtual void Notify(ProgressEvent e)
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
            }
        }

        /// <summary>
        /// Notifies that a file has been attached to a step.
        /// </summary>
        protected virtual void NotifyStepFileAttached(IStepInfo step, FileAttachment attachment)
        {
            if (StepWordAndStepNumberOnStart == StepWordAndStepNumberBehaviour.IncludeAsPrefix)
                _onNotify($"{GetIndentPrefix(step)}{GetStepWordAndStepNumber(step)}: 🔗{attachment.Name}: {attachment.FilePath}");
            else
                _onNotify($"{GetAnnotationIndent(step)}=> 🔗{attachment.Name}: {attachment.FilePath}");
        }

        /// <summary>
        /// Formats a description string for display, indenting each line.
        /// When <see cref="IndentLength"/> is positive, descriptions are indented to align with header content.
        /// </summary>
        protected virtual string FormatDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return string.Empty;
            var indent = IndentLength > 0 ? "          " : "  ";
            return $"{Environment.NewLine}{indent}{description.Replace(Environment.NewLine, Environment.NewLine + indent)}";
        }

        /// <summary>
        /// Formats labels as bracket-delimited tags, e.g. <c>[label1][label2] </c>.
        /// </summary>
        protected virtual string FormatLabels(IEnumerable<string> labels)
        {
            var joinedLabels = string.Join("][", labels);
            if (joinedLabels != string.Empty)
                joinedLabels = "[" + joinedLabels + "] ";
            return joinedLabels;
        }

        /// <summary>
        /// Builds a whitespace prefix for the given step.
        /// When <see cref="IndentLength"/> is 0 or less, returns a fixed two-space indent.
        /// When positive, returns a hierarchical indent based on the step's nesting depth.
        /// </summary>
        protected virtual string GetIndentPrefix(IStepInfo step)
        {
            if (IndentLength <= 0)
                return "  ";

            var parent = step.Parent;
            var prefix = IndentLength < 2 ? "  " : "";

            while (parent is IStepInfo parentStep)
            {
                prefix += new string(' ', IndentLength);
                parent = parentStep.Parent;
            }

            prefix += new string(' ', IndentLength);
            return prefix;
        }

        /// <summary>
        /// Builds the indent string used for step annotations (parameters, comments, file attachments)
        /// that appear indented under a step.
        /// </summary>
        protected virtual string GetAnnotationIndent(IStepInfo step)
        {
            return GetIndentPrefix(step) + new string(' ', Math.Max(IndentLength, 2));
        }

        /// <summary>
        /// Builds the step word and number string (e.g. "STEP 1" or "STEP 1/5").
        /// </summary>
        protected virtual string GetStepWordAndStepNumber(IStepInfo step)
        {
            return ShowFinalStepWithEachStep
                ? $"STEP {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}"
                : $"STEP {step.GroupPrefix}{step.Number}";
        }
    }
}