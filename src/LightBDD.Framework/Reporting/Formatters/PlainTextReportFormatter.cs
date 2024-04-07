using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LightBDD.Core.Formatting;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Reporting.Formatters
{
    /// <summary>
    /// Formats feature results as plain text.
    /// </summary>
    public class PlainTextReportFormatter : IReportFormatter
    {
        #region IReportFormatter Members

        /// <summary>
        /// Formats provided feature results and writes to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write formatted results to.</param>
        /// <param name="features">Feature results to format.</param>
        public void Format(Stream stream, params IFeatureResult[] features)
        {
            using (var writer = new StreamWriter(stream))
            {
                FormatSummary(writer, features);
                foreach (var feature in features)
                    FormatFeature(writer, feature);
            }
        }

        #endregion

        private static void FormatDetails(TextWriter writer, IScenarioResult scenario)
        {
            if (string.IsNullOrWhiteSpace(scenario.StatusDetails))
                return;

            writer.WriteLine("\t\tDetails:");
            writer.Write("\t\t\t");
            writer.Write(scenario.StatusDetails.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t\t\t"));
            writer.WriteLine();
        }

        private static void FormatFeature(TextWriter writer, IFeatureResult feature)
        {
            writer.WriteLine();
            writer.Write("Feature: ");
            writer.Write(feature.Info.Name);
            FormatLabels(writer, feature.Info.Labels);

            writer.WriteLine();

            if (!string.IsNullOrWhiteSpace(feature.Info.Description))
            {
                writer.Write("\t");
                writer.Write(feature.Info.Description.Replace(Environment.NewLine, Environment.NewLine + "\t"));
                writer.WriteLine();
            }

            foreach (var scenario in feature.GetScenariosOrderedByName())
                FormatScenario(writer, scenario);
        }

        private static void FormatLabels(TextWriter writer, IEnumerable<string> labels)
        {
            var first = true;

            foreach (var label in labels)
            {
                if (first)
                {
                    writer.Write(' ');
                    first = false;
                }
                writer.Write("[");
                writer.Write(label);
                writer.Write("]");
            }
        }

        private static void FormatScenario(TextWriter writer, IScenarioResult scenario)
        {
            writer.WriteLine();
            writer.Write("\tScenario: ");
            writer.Write(scenario.Info.Name);
            FormatLabels(writer, scenario.Info.Labels);
            writer.Write(" - ");
            writer.Write(scenario.Status);

            if (scenario.ExecutionTime != null)
            {
                writer.Write(" (");
                writer.Write(scenario.ExecutionTime.Duration.FormatPretty());
                writer.Write(")");
            }
            writer.WriteLine();

            if (!string.IsNullOrWhiteSpace(scenario.Info.Description))
            {
                writer.Write("\t\t");
                writer.Write(scenario.Info.Description.Replace(Environment.NewLine, Environment.NewLine + "\t\t"));
                writer.WriteLine();
            }

            if (scenario.Info.Categories.Any())
            {
                writer.Write("\t\tCategories: ");
                writer.WriteLine(string.Join(", ", scenario.Info.Categories));
            }

            writer.WriteLine();

            var commentBuilder = new StringBuilder();
            var attachmentBuilder = new StringBuilder();
            foreach (var step in scenario.GetSteps())
                FormatStep(writer, step, commentBuilder, attachmentBuilder);
            FormatDetails(writer, scenario);
            FormatComments(writer, commentBuilder);
            FormatAttachments(writer, attachmentBuilder);
        }

        private static void CollectComments(IStepResult step, StringBuilder commentBuilder)
        {
            foreach (var comment in step.Comments)
            {
                commentBuilder.Append("\t\t\tStep ").Append(step.Info.GroupPrefix).Append(step.Info.Number)
                    .Append(": ")
                    .AppendLine(comment.Replace(Environment.NewLine, Environment.NewLine + "\t\t\t\t"));
            }
        }

        private static void CollectAttachments(IStepResult step, StringBuilder attachmentBuilder)
        {
            foreach (var attachment in step.FileAttachments)
            {
                attachmentBuilder.Append("\t\t\tStep ").Append(step.Info.GroupPrefix).Append(step.Info.Number)
                    .Append(": ")
                    .Append(attachment.Name)
                    .Append(" - ")
                    .AppendLine(attachment.RelativePath);
            }
        }

        private static void FormatComments(TextWriter writer, StringBuilder commentBuilder)
        {
            if (commentBuilder.Length == 0)
                return;
            writer.WriteLine("\t\tComments:");
            writer.Write(commentBuilder);
        }

        private static void FormatAttachments(TextWriter writer, StringBuilder attachmentBuilder)
        {
            if (attachmentBuilder.Length == 0)
                return;
            writer.WriteLine("\t\tAttachments:");
            writer.Write(attachmentBuilder);
        }

        private static void FormatStep(TextWriter writer, IStepResult step, StringBuilder commentBuilder, StringBuilder attachmentBuilder, int indent = 0)
        {
            var stepIndent = new string('\t', indent + 2);
            writer.Write(stepIndent);
            writer.Write("Step ");
            writer.Write(step.Info.GroupPrefix);
            writer.Write(step.Info.Number);
            writer.Write(": ");
            writer.Write(step.Info.Name);
            writer.Write(" - ");
            writer.Write(step.Status);
            if (step.ExecutionTime != null)
            {
                writer.Write(" (");
                writer.Write(step.ExecutionTime.Duration.FormatPretty());
                writer.Write(")");
            }
            writer.WriteLine();
            foreach (var parameterResult in step.Parameters)
                FormatParameter(writer, parameterResult, stepIndent);
            CollectComments(step, commentBuilder);
            CollectAttachments(step, attachmentBuilder);
            foreach (var subStep in step.GetSubSteps())
                FormatStep(writer, subStep, commentBuilder, attachmentBuilder, indent + 1);
        }

        private static void FormatParameter(TextWriter writer, IParameterResult parameterResult, string stepIndent)
        {
            switch (parameterResult.Details)
            {
                case ITabularParameterDetails table:
                    writer.Write(stepIndent);
                    writer.Write(parameterResult.Name);
                    writer.WriteLine(":");
                    new TextTableRenderer(table).Render(writer, stepIndent);
                    writer.WriteLine();
                    break;
                case ITreeParameterDetails tree:
                    writer.Write(stepIndent);
                    writer.Write(parameterResult.Name);
                    writer.WriteLine(":");
                    TextTreeRenderer.Render(writer, stepIndent, tree);
                    writer.WriteLine();
                    break;
            }
        }

        private static void FormatSummary(TextWriter writer, IFeatureResult[] features)
        {
            var timeSummary = features.GetTestExecutionTimeSummary();

            writer.WriteLine("Summary:");
            var summary = new Dictionary<string, object>
            {
                {"Test execution start time", timeSummary.Start.ToString("yyyy-MM-dd HH:mm:ss UTC")},
                {"Test execution end time", timeSummary.End.ToString("yyyy-MM-dd HH:mm:ss UTC")},
                {"Test execution time", timeSummary.Duration.FormatPretty()},
                {"Test execution time (aggregated)", timeSummary.Aggregated.FormatPretty()},
                {"Number of features", features.Length},
                {"Number of scenarios", features.CountScenarios()},
                {"Passed scenarios", features.CountScenariosWithStatus(ExecutionStatus.Passed)},
                {"Bypassed scenarios", features.CountScenariosWithStatus(ExecutionStatus.Bypassed)},
                {"Failed scenarios", features.CountScenariosWithStatus(ExecutionStatus.Failed)},
                {"Ignored scenarios", features.CountScenariosWithStatus(ExecutionStatus.Ignored)},
                {"Number of steps", features.CountSteps()},
                {"Passed steps", features.CountStepsWithStatus(ExecutionStatus.Passed)},
                {"Bypassed steps", features.CountStepsWithStatus(ExecutionStatus.Bypassed)},
                {"Failed steps", features.CountStepsWithStatus(ExecutionStatus.Failed)},
                {"Ignored steps", features.CountStepsWithStatus(ExecutionStatus.Ignored)},
                {"Not Run steps", features.CountStepsWithStatus(ExecutionStatus.NotRun)}
            };

            var maxLength = summary.Keys.Max(k => k.Length);
            var format = $"\t{{0,-{maxLength}}}: {{1}}";
            foreach (var row in summary)
            {
                writer.Write(format, row.Key, row.Value);
                writer.WriteLine();
            }
        }
    }
}