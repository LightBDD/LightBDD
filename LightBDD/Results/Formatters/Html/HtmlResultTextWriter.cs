using System;
using System.Collections.Generic;
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

        private static IHtmlNode WriteExecutionSummary(IFeatureResult[] features)
        {
            return Html.Tag(Html5Tag.Section).Content(
                Html.Tag(HtmlTextWriterTag.H1).Content("Execution summary"),
                Html.Tag(Html5Tag.Article).Content(
                    Html.Tag(HtmlTextWriterTag.Table).Class("summary").Content(
                        GetKeyValueTableRow("Test execution start time:", features.GetTestExecutionStartTime().ToString("yyyy-MM-dd HH:mm:ss UTC")),
                        GetKeyValueTableRow("Test execution time:", features.GetTestExecutionTime().FormatPretty()),
                        GetKeyValueTableRow("Number of features:", features.Length.ToString(CultureInfo.InvariantCulture)),
                        GetKeyValueTableRow("Number of scenarios:", features.CountScenarios()),
                        GetKeyValueTableRow("Passed scenarios:", features.CountScenariosWithStatus(ResultStatus.Passed)),
                        GetKeyValueTableRow("Failed scenarios:", features.CountScenariosWithStatus(ResultStatus.Failed), "alert"),
                        GetKeyValueTableRow("Ignored scenarios:", features.CountScenariosWithStatus(ResultStatus.Ignored)),
                        GetKeyValueTableRow("Number of steps:", features.CountSteps()),
                        GetKeyValueTableRow("Passed steps:", features.CountStepsWithStatus(ResultStatus.Passed)),
                        GetKeyValueTableRow("Failed steps:", features.CountStepsWithStatus(ResultStatus.Failed), "alert"),
                        GetKeyValueTableRow("Ignored steps:", features.CountStepsWithStatus(ResultStatus.Ignored)),
                        GetKeyValueTableRow("Not Run steps:", features.CountStepsWithStatus(ResultStatus.NotRun))
                        )));
        }

        private static IHtmlNode GetKeyValueTableRow(string key, string value)
        {
            return Html.Tag(HtmlTextWriterTag.Tr).Content(
                Html.Tag(HtmlTextWriterTag.Th).Content(key),
                Html.Tag(HtmlTextWriterTag.Td).Content(value));
        }

        private static IHtmlNode GetKeyValueTableRow(string key, int value, string classNameIfNotZero = null)
        {
            var valueTag = Html.Tag(HtmlTextWriterTag.Td).Content(value.ToString(CultureInfo.InvariantCulture));

            if (classNameIfNotZero != null && value != 0)
                valueTag.Class(classNameIfNotZero);

            return Html.Tag(HtmlTextWriterTag.Tr).Content(
                Html.Tag(HtmlTextWriterTag.Th).Content(key),
                valueTag);
        }

        private static IHtmlNode WriteFeatureList(IFeatureResult[] features)
        {
            return Html.Tag(Html5Tag.Section).Content(
                Html.Tag(HtmlTextWriterTag.H1).Content("Feature summary"),
                Html.Tag(Html5Tag.Article).Content(
                    Html.Tag(HtmlTextWriterTag.Table).Class("features").Content(
                        GetSummaryTable(features))));
        }

        private static IEnumerable<IHtmlNode> GetSummaryTable(IFeatureResult[] features)
        {
            yield return GetSummaryTableHeaders("Feature", "Scenarios", "Passed", "Failed", "Ignored", "Steps", "Passed", "Failed", "Ignored", "Not Run", "Duration");
            for (int index = 0; index < features.Length; index++)
                yield return GetFeatureSummary(features[index], index + 1);
        }

        private static IHtmlNode GetFeatureSummary(IFeatureResult feature, int index)
        {
            return Html.Tag(HtmlTextWriterTag.Tr).Content(
                Html.Tag(HtmlTextWriterTag.Td).Content(
                    Html.Tag(HtmlTextWriterTag.A).Href("#feature" + index).Content(feature.Name),
                    GetLabel(feature.Label)),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.Scenarios.Count().ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Passed).ToString(CultureInfo.InvariantCulture)),
                GetNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountScenariosWithStatus(ResultStatus.Failed)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Ignored).ToString(CultureInfo.InvariantCulture)),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountSteps().ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Passed).ToString(CultureInfo.InvariantCulture)),
                GetNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountStepsWithStatus(ResultStatus.Failed)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Ignored).ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.NotRun).ToString(CultureInfo.InvariantCulture)),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.Scenarios.GetTestExecutionTime().FormatPretty()));
        }

        private static IHtmlNode GetNumericTagWithOptionalClass(HtmlTextWriterTag tag, string className, int value)
        {
            var node = Html.Tag(tag).Content(value.ToString(CultureInfo.InvariantCulture));
            if (value != 0)
                node.Class(className);
            return node;
        }

        private static IHtmlNode GetLabel(string label)
        {
            return Html.Tag(HtmlTextWriterTag.Span)
                .Class("label")
                .Content(string.IsNullOrWhiteSpace(label) ? string.Empty : string.Format("[{0}]", label.Trim()))
                .SpaceBefore()
                .SkipEmpty();
        }

        private static IHtmlNode GetSummaryTableHeaders(params string[] headers)
        {
            return Html.Tag(HtmlTextWriterTag.Tr)
                       .Content(headers.Select(header => Html.Tag(HtmlTextWriterTag.Th).Content(header)));
        }

        private static IHtmlNode WriteFeatureDetails(IFeatureResult[] features)
        {
            return Html.Tag(Html5Tag.Section).Content(
                GetFeatureDetailsContent(features));
        }

        private static IEnumerable<IHtmlNode> GetFeatureDetailsContent(IFeatureResult[] features)
        {
            yield return Html.Tag(HtmlTextWriterTag.H1).Content("Feature details");
            foreach (var htmlNode in GetFilterNodes())
                yield return htmlNode;

            for (var i = 0; i < features.Length; ++i)
                yield return GetFeatureDetails(features[i], i + 1);
        }

        private static IEnumerable<IHtmlNode> GetFilterNodes()
        {
            return new[]
            {
                Html.Tag(HtmlTextWriterTag.Span).Class("filter").Content("Filter:"),
                Html.Checkbox().Id("showPassed").Content("Passed").Checked().SpaceBefore(),
                Html.Checkbox().Id("showFailed").Content("Failed").Checked().SpaceBefore(),
                Html.Checkbox().Id("showIgnored").Content("Ignored").Checked().SpaceBefore(),
                Html.Checkbox().Id("showNotRun").Content("Not Run").Checked().SpaceBefore(),
                Html.Br()
            };
        }

        private static IHtmlNode GetFeatureDetails(IFeatureResult feature, int index)
        {
            return Html.Tag(Html5Tag.Article).Class(GetFeatureClasses(feature)).Content(
                Html.Checkbox().Class("toggle").Checked(),
                Html.Tag(HtmlTextWriterTag.Div).Class("header").Content(
                    Html.Tag(HtmlTextWriterTag.H2).Id("feature" + index).Class("title").Content(
                        Html.Text(feature.Name).Escape().Trim(),
                        GetLabel(feature.Label)),
                    Html.Tag(HtmlTextWriterTag.Div).Class("description").Content(feature.Description)),
                Html.Tag(HtmlTextWriterTag.Div).Class("scenarios").Content(
                    feature.Scenarios.Select(GetScenario)));
        }

        private static string GetFeatureClasses(IFeatureResult feature)
        {
            var builder = new StringBuilder("feature");
            foreach (var result in Enum.GetValues(typeof(ResultStatus)).Cast<ResultStatus>().Where(result => feature.CountScenariosWithStatus(result) > 0))
                builder.Append(" ").Append(GetStatusClass(result));

            if (!feature.Scenarios.Any())
                builder.Append(" ").Append(GetStatusClass(ResultStatus.NotRun));

            return builder.ToString();
        }

        private static IHtmlNode GetScenario(IScenarioResult scenario)
        {
            return Html.Tag(HtmlTextWriterTag.Div).Class("scenario " + GetStatusClass(scenario.Status)).Content(
                Html.Checkbox().Class("toggle").Checked(),
                Html.Tag(HtmlTextWriterTag.H3).Class("title").Content(
                    GetStatus(scenario.Status),
                    Html.Text(scenario.Name).Escape().Trim(),
                    GetLabel(scenario.Label),
                    GetDuration(scenario.ExecutionTime)),
                Html.Tag(HtmlTextWriterTag.Div).Content(scenario.Steps.Select(GetStep)),
                Html.Tag(HtmlTextWriterTag.Div).Class("details").Content(scenario.StatusDetails).SkipEmpty());
        }

        private static IHtmlNode GetDuration(TimeSpan? executionTime)
        {
            return Html.Tag(HtmlTextWriterTag.Span)
                .Class("duration")
                .Content(executionTime != null ? string.Format("({0})", executionTime.FormatPretty()) : string.Empty)
                .SkipEmpty()
                .SpaceBefore();
        }

        private static IHtmlNode GetStatus(ResultStatus status)
        {
            return Html.Tag(HtmlTextWriterTag.Span)
                .Class("status " + GetStatusClass(status))
                .Content(status.ToString())
                .SpaceAfter();
        }

        private static IHtmlNode GetStep(IStepResult step)
        {
            return Html.Tag(HtmlTextWriterTag.Div).Class("step").Content(
                GetStatus(step.Status),
                Html.Text(string.Format("{0}. {1}", step.Number, step.Name)).Escape().Trim(),
                GetDuration(step.ExecutionTime));
        }

        private static string GetStatusClass(ResultStatus status)
        {
            return status.ToString().ToLowerInvariant();
        }

        public void Write(IFeatureResult[] features)
        {
            _writer.NewLine = "";

            _writer
                .WriteTag(Html.Text("<!DOCTYPE HTML>"))
                .WriteTag(Html.Tag(HtmlTextWriterTag.Html).Content(
                    Html.Tag(HtmlTextWriterTag.Head).Content(
                        Html.Tag(HtmlTextWriterTag.Meta).Attribute("charset", "UTF-8"),
                        Html.Tag(HtmlTextWriterTag.Title).Content("Summary"),
                        Html.Tag(HtmlTextWriterTag.Style).Content(_styles, false, false)),
                    Html.Tag(HtmlTextWriterTag.Body).Content(
                        WriteExecutionSummary(features),
                        WriteFeatureList(features),
                        WriteFeatureDetails(features)
                        )))
                .Flush();
        }
    }
}