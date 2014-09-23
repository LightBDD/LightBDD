using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using LightBDD.Formatters;

namespace LightBDD.Results.Formatters
{
    /// <summary>
    /// Formats feature results as HTML.
    /// </summary>
    public class HtmlResultFormatter : IResultFormatter
    {
        private class Html5Tag
        {
            public const string Section = "section";
        }
        private readonly string _styles;

        /// <summary>
        /// Formatter constructor.
        /// </summary>
        public HtmlResultFormatter()
        {
            _styles = ReadStyles();
        }

        private string ReadStyles()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream("LightBDD.Results.Formatters.styles.css"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Formats feature results.
        /// </summary>
        /// <param name="features">Features to format.</param>
        public string Format(params IFeatureResult[] features)
        {
            using (var memory = new MemoryStream())
            using (var stream = new StreamWriter(memory))
            using (var writer = new HtmlTextWriter(stream, ""))
            {
                writer.NewLine = "";
                WriteHeader(writer);
                WriteExecutionSummary(writer, features);
                WriteFeatureList(writer, features);
                WriteFeatureDetails(writer, features);
                WriteFooter(writer);
                writer.Flush();
                return Encoding.Default.GetString(memory.ToArray());
            }
        }

        private void WriteExecutionSummary(HtmlTextWriter writer, IFeatureResult[] features)
        {
            writer.RenderBeginTag(Html5Tag.Section);
            WriteTag(writer, HtmlTextWriterTag.H1, null, "Execution summary");
            WriteTag(writer, HtmlTextWriterTag.Table, "summary", () =>
            {
                WriteKeyValueTableRow(writer, "Test execution start time:", (features.GetTestExecutionStartTime() ?? DateTimeOffset.UtcNow).ToString("yyyy-MM-dd HH:mm:ss UTC"));
                WriteKeyValueTableRow(writer, "Test execution time:", features.GetTestExecutionTime().FormatPretty());
                WriteKeyValueTableRow(writer, "Number of features:", features.Length.ToString(CultureInfo.InvariantCulture));
                WriteKeyValueTableRow(writer, "Number of scenarios:", features.SelectMany(f => f.Scenarios).Count());
                WriteKeyValueTableRow(writer, "Passed scenarios:", features.SelectMany(f => f.Scenarios).Count(s => s.Status == ResultStatus.Passed));
                WriteKeyValueTableRow(writer, "Ignored scenarios:", features.SelectMany(f => f.Scenarios).Count(s => s.Status == ResultStatus.Ignored));
                WriteKeyValueTableRow(writer, "Failed scenarios:", features.SelectMany(f => f.Scenarios).Count(s => s.Status == ResultStatus.Failed), "alert");
            });
            writer.RenderEndTag();
        }

        private void WriteKeyValueTableRow(HtmlTextWriter writer, string key, int value, string classNameIfNotZero = null)
        {
            WriteKeyValueTableRow(writer, key, value.ToString(CultureInfo.InvariantCulture), value != 0 ? classNameIfNotZero : null);
        }

        private void WriteKeyValueTableRow(HtmlTextWriter writer, string key, string value, string className = null)
        {
            WriteTag(writer, HtmlTextWriterTag.Tr, null, () =>
            {
                WriteTag(writer, HtmlTextWriterTag.Th, null, key);
                WriteTag(writer, HtmlTextWriterTag.Td, className, value);
            });
        }

        private void WriteFeatureList(HtmlTextWriter writer, IFeatureResult[] features)
        {
            writer.RenderBeginTag(Html5Tag.Section);
            WriteTag(writer, HtmlTextWriterTag.H1, null, "Feature summary");
            WriteTag(writer, HtmlTextWriterTag.Table, "features", () =>
            {
                WriteTableHeaders(writer, "Feature", "Scenarios", "Passed", "Ignored", "Failed", "Duration");
                foreach (var feature in features)
                {
                    WriteTag(writer, HtmlTextWriterTag.Tr, null, () =>
                    {
                        WriteTag(writer, HtmlTextWriterTag.Td, null, () =>
                        {
                            WriteTag(writer, HtmlTextWriterTag.Span, "label", feature.Label);
                            writer.WriteEncodedText(feature.Name);
                        });
                        WriteTag(writer, HtmlTextWriterTag.Td, null, feature.Scenarios.Count().ToString(CultureInfo.InvariantCulture));
                        WriteTag(writer, HtmlTextWriterTag.Td, null, feature.Scenarios.Count(s => s.Status == ResultStatus.Passed).ToString(CultureInfo.InvariantCulture));
                        WriteTag(writer, HtmlTextWriterTag.Td, null, feature.Scenarios.Count(s => s.Status == ResultStatus.Ignored).ToString(CultureInfo.InvariantCulture));
                        WriteNumericTagWithOptionalClass(writer, HtmlTextWriterTag.Td, "alert", feature.Scenarios.Count(s => s.Status == ResultStatus.Failed));
                        WriteTag(writer, HtmlTextWriterTag.Td, null, feature.Scenarios.GetTestExecutionTime().FormatPretty());
                    });
                }
            });
            writer.RenderEndTag();
        }

        private void WriteNumericTagWithOptionalClass(HtmlTextWriter writer, HtmlTextWriterTag tag, string classNameIfNotZero, int value)
        {
            WriteTag(writer, tag, value != 0 ? classNameIfNotZero : null, value.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteTableHeaders(HtmlTextWriter writer, params string[] headers)
        {
            WriteTag(writer, HtmlTextWriterTag.Tr, null, () =>
            {
                foreach (var header in headers)
                    WriteTag(writer, HtmlTextWriterTag.Th, null, header);
            });
        }

        private void WriteFooter(HtmlTextWriter writer)
        {
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private void WriteHeader(HtmlTextWriter writer)
        {
            writer.WriteLine("<!DOCTYPE HTML>");
            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            writer.RenderBeginTag(HtmlTextWriterTag.Head);

            writer.AddAttribute("charset", "UTF-8");
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();

            WriteTag(writer, HtmlTextWriterTag.Title, null, "Summary");

            WriteTag(writer, HtmlTextWriterTag.Style, null, _styles, false);

            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Body);
        }

        private void WriteFeatureDetails(HtmlTextWriter writer, IEnumerable<IFeatureResult> features)
        {
            writer.RenderBeginTag(Html5Tag.Section);
            WriteTag(writer, HtmlTextWriterTag.H1, null, "Feature details");
            foreach (var feature in features)
                WriteFeature(writer, feature);
            writer.RenderEndTag();
        }

        private void WriteFeature(HtmlTextWriter writer, IFeatureResult feature)
        {
            WriteTag(writer, HtmlTextWriterTag.Div, "feature", () =>
            {
                WriteTag(writer, HtmlTextWriterTag.Div, "header", () =>
                {
                    WriteTag(writer, HtmlTextWriterTag.Div, "title", () =>
                    {
                        WriteTag(writer, HtmlTextWriterTag.Span, "label", feature.Label);
                        writer.WriteEncodedText(feature.Name);
                    });
                    WriteTag(writer, HtmlTextWriterTag.Div, "description", feature.Description);
                });
                WriteTag(writer, HtmlTextWriterTag.Div, "scenarios", () =>
                {
                    foreach (var scenario in feature.Scenarios)
                        WriteScenario(writer, scenario);
                });
            });
        }

        private void WriteScenario(HtmlTextWriter writer, IScenarioResult scenario)
        {
            WriteTag(writer, HtmlTextWriterTag.Div, "scenario", () =>
            {
                WriteTag(writer, HtmlTextWriterTag.Div, "title", () =>
                {
                    WriteStatus(writer, scenario.Status);
                    WriteTag(writer, HtmlTextWriterTag.Span, "label", scenario.Label);
                    writer.WriteEncodedText(scenario.Name);
                    if (scenario.ExecutionTime != null)
                        WriteTag(writer, HtmlTextWriterTag.Span, "duration", string.Format(" ({0})", scenario.ExecutionTime.FormatPretty()));
                });
                foreach (var step in scenario.Steps)
                    WriteStep(writer, step);
                WriteTag(writer, HtmlTextWriterTag.Div, "details", scenario.StatusDetails);
            });
        }

        private void WriteStep(HtmlTextWriter writer, IStepResult step)
        {
            WriteTag(writer, HtmlTextWriterTag.Div, "step", () =>
            {
                WriteStatus(writer, step.Status);
                WriteTag(writer, HtmlTextWriterTag.Span, "number", string.Format("{0}.", step.Number));
                writer.WriteEncodedText(step.Name);
                if (step.ExecutionTime != null)
                    WriteTag(writer, HtmlTextWriterTag.Span, "duration", string.Format(" ({0})", step.ExecutionTime.FormatPretty()));
            });
        }

        private static void WriteStatus(HtmlTextWriter writer, ResultStatus status)
        {
            WriteTag(writer, HtmlTextWriterTag.Div, "status " + status.ToString().ToLowerInvariant(), status.ToString());
        }

        private void WriteTag(HtmlTextWriter writer, HtmlTextWriterTag tag, string className, Action contentRenderer)
        {
            if (className != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, className);
            writer.RenderBeginTag(tag);
            contentRenderer();
            writer.RenderEndTag();
        }

        private static void WriteTag(HtmlTextWriter writer, HtmlTextWriterTag tag, string className, string content, bool escapeContent = true)
        {
            if (content == null)
                return;
            if (className != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, className);
            writer.RenderBeginTag(tag);
            if (escapeContent)
                writer.WriteEncodedText(content.Trim());
            else
                writer.Write(content.Trim());
            writer.RenderEndTag();
        }
    }
}