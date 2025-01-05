using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Reporting.Formatters;

/// <summary>
/// Formats feature results as markdown.
/// </summary>
public class MarkdownReportFormatter : IReportFormatter
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
            writer.WriteLine("# Features");
            foreach (var feature in features)
                FormatFeature(writer, feature);
        }
    }

    #endregion

    private static void FormatDetails(TextWriter writer, IScenarioResult scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario.StatusDetails))
            return;

        writer.WriteLine("> [!IMPORTANT]");
        writer.WriteLine("> <pre>");
        writer.WriteLine("> " + scenario.StatusDetails.Trim().Replace(Environment.NewLine, $"{Environment.NewLine}> "));
        writer.WriteLine("> </pre>");
        writer.WriteLine();
    }

    private static void FormatFeature(TextWriter writer, IFeatureResult feature)
    {
        writer.WriteLine();
        writer.WriteLine();
        writer.Write($"## {feature.Info.Name}");
        FormatLabels(writer, feature.Info.Labels);

        writer.WriteLine();

        if (!string.IsNullOrWhiteSpace(feature.Info.Description))
        {
            writer.Write("> ");
            writer.Write(feature.Info.Description.Replace(Environment.NewLine, Environment.NewLine + "> "));
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
                writer.Write(" :label:");
                first = false;
            }

            writer.Write($"`{label}`");
        }
    }

    private static void FormatScenario(TextWriter writer, IScenarioResult scenario)
    {
        writer.WriteLine();
        writer.WriteLine();
        writer.Write($"### {GetStatus(scenario.Status)} Scenario: {scenario.Info.Name.Format(MarkdownStepNameDecorator.Instance)}");
        FormatLabels(writer, scenario.Info.Labels);
        if (scenario.Info.Categories.Any())
            writer.Write($" :file_folder:{string.Join("", scenario.Info.Categories.Select(c => $"`{c}`"))}");
        if (scenario.ExecutionTime != null)
            WriteExecutionTime(writer, scenario.ExecutionTime);
        writer.WriteLine();

        if (!string.IsNullOrWhiteSpace(scenario.Info.Description))
        {
            writer.Write("> ");
            writer.Write(scenario.Info.Description.Replace(Environment.NewLine, Environment.NewLine + "> "));
            writer.WriteLine();
            writer.WriteLine();
        }

        var commentBuilder = new StringBuilder();
        var attachmentBuilder = new StringBuilder();
        foreach (var step in scenario.GetSteps())
            FormatStep(writer, step, commentBuilder, attachmentBuilder);
        FormatDetails(writer, scenario);
        FormatComments(writer, commentBuilder);
        FormatAttachments(writer, attachmentBuilder);
        writer.WriteLine("---");
        writer.WriteLine("");
        writer.WriteLine("");
    }

    private static void WriteExecutionTime(TextWriter writer, ExecutionTime executionTime)
    {
        writer.Write($" :watch:`{executionTime.Duration.FormatPretty()}`");
    }

    private static string GetStatus(ExecutionStatus status)
    {
        return status switch
        {
            ExecutionStatus.Passed => ":white_check_mark:",
            ExecutionStatus.NotRun => ":white_circle:",
            ExecutionStatus.Bypassed => ":large_blue_diamond:",
            ExecutionStatus.Ignored => ":warning:",
            ExecutionStatus.Failed => ":red_circle:",
            _ => status.ToString().ToLowerInvariant()
        };
    }

    private static void CollectComments(IStepResult step, StringBuilder commentBuilder)
    {
        foreach (var comment in step.Comments)
        {
            commentBuilder.Append("> Step ").Append(step.Info.GroupPrefix).Append(step.Info.Number).Append(": ")
                .AppendLine(comment.Trim().Replace(Environment.NewLine, $"{Environment.NewLine}> "));
        }
    }

    private static void CollectAttachments(IStepResult step, StringBuilder attachmentBuilder)
    {
        var first = true;
        foreach (var attachment in step.FileAttachments)
        {
            if (first)
            {
                attachmentBuilder.Append("Step ").Append(step.Info.GroupPrefix).Append(step.Info.Number)
                    .Append(": ");
                first = false;
            }
            else
                attachmentBuilder.Append(", ");

            attachmentBuilder.Append($"[:link: {attachment.Name}]({attachment.RelativePath})");
        }
    }

    private static void FormatComments(TextWriter writer, StringBuilder commentBuilder)
    {
        if (commentBuilder.Length == 0)
            return;
        writer.WriteLine("> [!NOTE]");
        writer.WriteLine("> <pre>");
        writer.Write(commentBuilder);
        writer.WriteLine("> </pre>");
        writer.WriteLine();
    }

    private static void FormatAttachments(TextWriter writer, StringBuilder attachmentBuilder)
    {
        if (attachmentBuilder.Length == 0)
            return;
        writer.WriteLine("**Attachments:**  ");
        writer.Write(attachmentBuilder);
    }

    private static void FormatStep(TextWriter writer, IStepResult step, StringBuilder commentBuilder, StringBuilder attachmentBuilder, int intent = 1)
    {
        var intentString = intent == 0 ? string.Empty : (string.Join("", Enumerable.Repeat("&nbsp;", intent * 3)) + " ");
        writer.Write($"#### {intentString}{GetStatus(step.Status)} Step {step.Info.GroupPrefix}{step.Info.Number}: {step.Info.Name.Format(MarkdownStepNameDecorator.Instance)}");

        if (step.ExecutionTime != null)
        {
            WriteExecutionTime(writer, step.ExecutionTime);
        }

        writer.WriteLine();
        foreach (var parameterResult in step.Parameters)
            FormatParameter(writer, parameterResult);
        CollectComments(step, commentBuilder);
        CollectAttachments(step, attachmentBuilder);
        foreach (var subStep in step.GetSubSteps())
            FormatStep(writer, subStep, commentBuilder, attachmentBuilder, intent + 1);
    }

    private static void FormatParameter(TextWriter writer, IParameterResult parameterResult)
    {
        switch (parameterResult.Details)
        {
            case ITabularParameterDetails table:
                writer.WriteLine($"**${parameterResult.Name}**:");
                new MarkdownTableRenderer(table).Render(writer);
                writer.WriteLine();
                writer.WriteLine();
                break;
            case ITreeParameterDetails tree:
                writer.WriteLine($"**${parameterResult.Name}**:");
                MarkdownTreeRenderer.Render(writer, tree);
                writer.WriteLine();
                writer.WriteLine();
                break;
        }
    }

    private static void FormatSummary(TextWriter writer, IFeatureResult[] features)
    {
        var timeSummary = features.GetTestExecutionTimeSummary();

        writer.WriteLine("# Summary");
        writer.WriteLine();

        var executionStatus = features.SelectMany(f => f.GetScenarios()).Select(s => s.Status).OrderByDescending(x => x).DefaultIfEmpty(ExecutionStatus.NotRun).First();
        if (executionStatus != ExecutionStatus.Failed)
            executionStatus = ExecutionStatus.Passed;

        var summary = new (string key, object value)[]
        {
            ("Execution Start", timeSummary.Start.ToString("yyyy-MM-dd HH:mm:ss UTC")),
            ("Execution Duration", timeSummary.Duration.FormatPretty()),
            ("**Overall Status**", $"{GetStatus(executionStatus)} {executionStatus.ToString()}"),
            ("Total Features", features.Length),
            ("Total Scenarios", features.CountScenarios()),
            ("Passed Scenarios", features.CountScenariosWithStatus(ExecutionStatus.Passed)),
            ("Bypassed Scenarios", features.CountScenariosWithStatus(ExecutionStatus.Bypassed)),
            ("Failed Scenarios", features.CountScenariosWithStatus(ExecutionStatus.Failed)),
            ("Ignored Scenarios", features.CountScenariosWithStatus(ExecutionStatus.Ignored)),
            ("Total Steps", features.CountSteps()),
            ("Passed Steps", features.CountStepsWithStatus(ExecutionStatus.Passed)),
            ("Bypassed Steps", features.CountStepsWithStatus(ExecutionStatus.Bypassed)),
            ("Failed Steps", features.CountStepsWithStatus(ExecutionStatus.Failed)),
            ("Ignored Steps", features.CountStepsWithStatus(ExecutionStatus.Ignored)),
            ("Not Run Steps", features.CountStepsWithStatus(ExecutionStatus.NotRun))
        };
        summary = summary.Where(p => p.value is not 0).ToArray();

        var maxLength = summary.Max(k => k.key.Length);

        writer.WriteLine("| Title | Value |");
        writer.WriteLine("| ----: | :---- |");

        var format = $"| {{0,-{maxLength}}} | {{1}} |";
        foreach (var row in summary)
        {
            writer.Write(format, row.key, row.value);
            writer.WriteLine();
        }
    }
}