using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using LightBDD.Formatters;

namespace LightBDD.Results.Formatters.Html
{
    internal class HtmlResultTextWriter : IDisposable
    {
        private readonly HtmlTextWriter _writer;
        private readonly string _styles = ReadStyles();
        public HtmlResultTextWriter(Stream outputStream)
        {
            _writer = new HtmlTextWriter(new StreamWriter(outputStream), "");
        }

        private static string ReadStyles()
        {
            using (var stream = typeof(HtmlResultTextWriter).Assembly.GetManifestResourceStream("LightBDD.Results.Formatters.styles.css"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private void WriteExecutionSummary(IFeatureResult[] features)
        {
            _writer.WriteTag(Html5Tag.Section, null, () =>
            {
                _writer.WriteTag(HtmlTextWriterTag.H1, null, "Execution summary");
                _writer.WriteTag(Html5Tag.Article, null, () =>
                    _writer.WriteTag(HtmlTextWriterTag.Table, "summary", () =>
                    {
                        WriteKeyValueTableRow("Test execution start time:", (features.GetTestExecutionStartTime() ?? DateTimeOffset.UtcNow).ToString("yyyy-MM-dd HH:mm:ss UTC"));
                        WriteKeyValueTableRow("Test execution time:", features.GetTestExecutionTime().FormatPretty());
                        WriteKeyValueTableRow("Number of features:", features.Length.ToString(CultureInfo.InvariantCulture));
                        WriteKeyValueTableRow("Number of scenarios:", features.SelectMany(f => f.Scenarios).Count());
                        WriteKeyValueTableRow("Passed scenarios:", features.Sum(f => f.CountScenarios(ResultStatus.Passed)));
                        WriteKeyValueTableRow("Ignored scenarios:", features.Sum(f => f.CountScenarios(ResultStatus.Ignored)));
                        WriteKeyValueTableRow("Failed scenarios:", features.Sum(f => f.CountScenarios(ResultStatus.Failed)), "alert");
                    }));
            });
        }

        private void WriteKeyValueTableRow(string key, int value, string classNameIfNotZero = null)
        {
            WriteKeyValueTableRow(key, value.ToString(CultureInfo.InvariantCulture), value != 0 ? classNameIfNotZero : null);
        }

        private void WriteKeyValueTableRow(string key, string value, string className = null)
        {
            _writer.WriteTag(HtmlTextWriterTag.Tr, null, () =>
            {
                _writer.WriteTag(HtmlTextWriterTag.Th, null, key);
                _writer.WriteTag(HtmlTextWriterTag.Td, className, value);
            });
        }

        private void WriteFeatureList(IFeatureResult[] features)
        {
            _writer.WriteTag(Html5Tag.Section, null, () =>
            {
                _writer.WriteTag(HtmlTextWriterTag.H1, null, "Feature summary");
                _writer.WriteTag(Html5Tag.Article, null, () =>
                    _writer.WriteTag(HtmlTextWriterTag.Table, "features", () =>
                        {
                            WriteTableHeaders("Feature", "Scenarios", "Passed", "Ignored", "Failed", "Duration");
                            for (int index = 0; index < features.Length; index++)
                                WriteFeatureSummary(features[index], index + 1);
                        }));
            });
        }

        private void WriteFeatureSummary(IFeatureResult feature, int index)
        {
            _writer.WriteTag(HtmlTextWriterTag.Tr, null, () =>
            {
                _writer.WriteTag(HtmlTextWriterTag.Td, null, () =>
                {
                    WriteLabel(feature.Label);
                    _writer.WriteLink("#feature" + index, feature.Name);
                });
                _writer.WriteTag(HtmlTextWriterTag.Td, null, feature.Scenarios.Count().ToString(CultureInfo.InvariantCulture));
                _writer.WriteTag(HtmlTextWriterTag.Td, null, feature.CountScenarios(ResultStatus.Passed).ToString(CultureInfo.InvariantCulture));
                _writer.WriteTag(HtmlTextWriterTag.Td, null, feature.CountScenarios(ResultStatus.Ignored).ToString(CultureInfo.InvariantCulture));
                WriteNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountScenarios(ResultStatus.Failed));
                _writer.WriteTag(HtmlTextWriterTag.Td, null, feature.Scenarios.GetTestExecutionTime().FormatPretty());
            });
        }

        private void WriteNumericTagWithOptionalClass(HtmlTextWriterTag tag, string classNameIfNotZero, int value)
        {
            _writer.WriteTag(tag, value != 0 ? classNameIfNotZero : null, value.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteTableHeaders(params string[] headers)
        {
            _writer.WriteTag(HtmlTextWriterTag.Tr, null, () =>
            {
                foreach (var header in headers)
                    _writer.WriteTag(HtmlTextWriterTag.Th, null, header);
            });
        }

        private void WriteFooter()
        {
            _writer.RenderEndTag();
            _writer.RenderEndTag();
        }

        private void WriteHeader()
        {
            _writer.WriteLine("<!DOCTYPE HTML>");
            _writer.RenderBeginTag(HtmlTextWriterTag.Html);
            _writer.RenderBeginTag(HtmlTextWriterTag.Head);

            _writer.AddAttribute("charset", "UTF-8");
            _writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            _writer.RenderEndTag();

            _writer.WriteTag(HtmlTextWriterTag.Title, null, "Summary");

            _writer.WriteTag(HtmlTextWriterTag.Style, null, _styles, false);

            _writer.RenderEndTag();
            _writer.RenderBeginTag(HtmlTextWriterTag.Body);
        }

        private void WriteFeatureDetails(IFeatureResult[] features)
        {
            _writer.WriteTag(Html5Tag.Section, null, () =>
            {
                _writer.WriteTag(HtmlTextWriterTag.H1, null, "Feature details");
                WriteStatusFilter();
                for (var i = 0; i < features.Length; ++i)
                    WriteFeature(features[i], i + 1);
            });
        }

        private void WriteStatusFilter()
        {
            _writer.WriteTag(HtmlTextWriterTag.Span, "filter", "Filter:")
                   .WriteSpace().WriteCheckbox("showPassed", "Passed", true)
                   .WriteSpace().WriteCheckbox("showFailed", "Failed", true)
                   .WriteSpace().WriteCheckbox("showIgnored", "Ignored", true)
                   .WriteSpace().WriteCheckbox("showNotRun", "Not Run", true)
                   .WriteBreak();
        }

        private void WriteFeature(IFeatureResult feature, int index)
        {
            _writer.WriteTag(Html5Tag.Article, GetFeatureClasses(feature), () =>
                {
                    _writer.WriteCheckbox("toggle",true);
                _writer.WriteTag(HtmlTextWriterTag.Div, "header", () =>
                {
                    _writer.AddAttribute(HtmlTextWriterAttribute.Id, "feature" + index);
                    _writer.WriteTag(HtmlTextWriterTag.H2, "title", () =>
                    {
                        WriteLabel(feature.Label);
                        _writer.WriteEncodedText(feature.Name);
                    });
                    _writer.WriteTag(HtmlTextWriterTag.Div, "description", feature.Description);
                });
                _writer.WriteTag(HtmlTextWriterTag.Div, "scenarios", () =>
                {
                    foreach (var scenario in feature.Scenarios)
                        WriteScenario(scenario);
                });
            });
        }

        private static string GetFeatureClasses(IFeatureResult feature)
        {
            var builder = new StringBuilder("feature");
            foreach (var result in Enum.GetValues(typeof(ResultStatus)).Cast<ResultStatus>().Where(result => feature.CountScenarios(result) > 0))
                builder.Append(" ").Append(GetStatusClass(result));

            if (!feature.Scenarios.Any())
                builder.Append(" ").Append(GetStatusClass(ResultStatus.NotRun));

            return builder.ToString();
        }

        private void WriteScenario(IScenarioResult scenario)
        {
            _writer.WriteTag(HtmlTextWriterTag.Div, "scenario " + GetStatusClass(scenario.Status), () =>
            {
                _writer.WriteCheckbox("toggle", true);
                _writer.WriteTag(HtmlTextWriterTag.H3, "title", () =>
                {
                    WriteStatus(scenario.Status);
                    WriteLabel(scenario.Label);
                    _writer.WriteEncodedText(scenario.Name);
                    WriteDuration(scenario.ExecutionTime);
                });
                
                foreach (var step in scenario.Steps)
                    WriteStep(step);
                _writer.WriteTag(HtmlTextWriterTag.Div, "details", scenario.StatusDetails);
            });
        }

        private void WriteLabel(string label)
        {
            if (label == null)
                return;
            _writer.WriteTag(HtmlTextWriterTag.Span, "label", label)
                .WriteSpace();
        }

        private void WriteStep(IStepResult step)
        {
            _writer.WriteTag(HtmlTextWriterTag.Div, "step", () =>
            {
                WriteStatus(step.Status);
                _writer.WriteEncodedText(string.Format("{0}. {1}", step.Number, step.Name));
                WriteDuration(step.ExecutionTime);
            });
        }

        private void WriteDuration(TimeSpan? executionTime)
        {
            if (executionTime != null)
                _writer.WriteSpace().WriteTag(HtmlTextWriterTag.Span, "duration", string.Format("({0})", executionTime.FormatPretty()));
        }

        private void WriteStatus(ResultStatus status)
        {
            _writer.WriteTag(HtmlTextWriterTag.Span, "status " + GetStatusClass(status), status.ToString())
                   .WriteSpace();
        }

        private static string GetStatusClass(ResultStatus status)
        {
            return status.ToString().ToLowerInvariant();
        }

        public void Write(IFeatureResult[] features)
        {
            _writer.NewLine = "";
            WriteHeader();
            WriteExecutionSummary(features);
            WriteFeatureList(features);
            WriteFeatureDetails(features);
            WriteFooter();
            _writer.Flush();
        }
    }
}