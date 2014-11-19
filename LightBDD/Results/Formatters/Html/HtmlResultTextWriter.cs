using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using LightBDD.Formatters;
using LightBDD.Naming;

namespace LightBDD.Results.Formatters.Html
{
    internal class HtmlResultTextWriter : IDisposable
    {
        private static readonly IStepNameDecorator _stepNameDecorator = new HtmlStepNameDecorator();
        private readonly HtmlTextWriter _writer;
        private readonly string _styles = ReadResource("LightBDD.Results.Formatters.Html.styles.css");
        private readonly string _scripts = ReadResource("LightBDD.Results.Formatters.Html.scripts.js");
        public HtmlResultTextWriter(Stream outputStream)
        {
            _writer = new HtmlTextWriter(new StreamWriter(outputStream), "");
        }

        private static string ReadResource(string path)
        {
            using (var stream = typeof(HtmlResultTextWriter).Assembly.GetManifestResourceStream(path))
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
                        GetKeyValueTableRow("Bypassed scenarios:", features.CountScenariosWithStatus(ResultStatus.Bypassed)),
                        GetKeyValueTableRow("Failed scenarios:", features.CountScenariosWithStatus(ResultStatus.Failed), "alert"),
                        GetKeyValueTableRow("Ignored scenarios:", features.CountScenariosWithStatus(ResultStatus.Ignored)),
                        GetKeyValueTableRow("Number of steps:", features.CountSteps()),
                        GetKeyValueTableRow("Passed steps:", features.CountStepsWithStatus(ResultStatus.Passed)),
                        GetKeyValueTableRow("Bypassed steps:", features.CountStepsWithStatus(ResultStatus.Bypassed)),
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
                    Html.Tag(HtmlTextWriterTag.Table).Id("featuresSummary").Class("features").Content(
                        GetSummaryTable(features))));
        }

        private static IEnumerable<IHtmlNode> GetSummaryTable(IFeatureResult[] features)
        {
            var sortable = "sortable";
            var sortableMinor = "sortable minor";
            var hidden = "hidden";

            yield return GetSummaryTableHeaders(
                Tuple.Create("Feature", sortable, "sortTable('featuresSummary',0,false,this)"),
                Tuple.Create("Scenarios", sortable, "sortTable('featuresSummary',1,true,this)"),
                Tuple.Create("Passed", sortableMinor, "sortTable('featuresSummary',2,true,this)"),
                Tuple.Create("Bypassed", sortableMinor, "sortTable('featuresSummary',3,true,this)"),
                Tuple.Create("Failed", sortableMinor, "sortTable('featuresSummary',4,true,this)"),
                Tuple.Create("Ignored", sortableMinor, "sortTable('featuresSummary',5,true,this)"),
                Tuple.Create("Steps", sortable, "sortTable('featuresSummary',6,true,this)"),
                Tuple.Create("Passed", sortableMinor, "sortTable('featuresSummary',7,true,this)"),
                Tuple.Create("Bypassed", sortableMinor, "sortTable('featuresSummary',8,true,this)"),
                Tuple.Create("Failed", sortableMinor, "sortTable('featuresSummary',9,true,this)"),
                Tuple.Create("Ignored", sortableMinor, "sortTable('featuresSummary',10,true,this)"),
                Tuple.Create("Not Run", sortableMinor, "sortTable('featuresSummary',11,true,this)"),
                Tuple.Create("Duration", sortable, "sortTable('featuresSummary',13,true,this)"),
                Tuple.Create("", hidden, ""),
                Tuple.Create("Average", sortableMinor, "sortTable('featuresSummary',15,true,this)"),
                Tuple.Create("", hidden, "")
                );
            yield return Html.Tag(HtmlTextWriterTag.Tbody).Content(features.Select((t, index) => GetFeatureSummary(t, index + 1)));

        }

        private static IHtmlNode GetFeatureSummary(IFeatureResult feature, int index)
        {
            var testExecutionTime = feature.Scenarios.GetTestExecutionTime();
            var testAverageExecutionTime = feature.Scenarios.GetTestAverageExecutionTime();

            return Html.Tag(HtmlTextWriterTag.Tr).Content(
                Html.Tag(HtmlTextWriterTag.Td).Content(
                    Html.Tag(HtmlTextWriterTag.A).Href("#feature" + index).Content(feature.Name),
                    GetLabel(feature.Label)),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.Scenarios.Count().ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Passed).ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Bypassed).ToString(CultureInfo.InvariantCulture)),
                GetNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountScenariosWithStatus(ResultStatus.Failed)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Ignored).ToString(CultureInfo.InvariantCulture)),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountSteps().ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Passed).ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Bypassed).ToString(CultureInfo.InvariantCulture)),
                GetNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountStepsWithStatus(ResultStatus.Failed)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Ignored).ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.NotRun).ToString(CultureInfo.InvariantCulture)),

                Html.Tag(HtmlTextWriterTag.Td).Content(testExecutionTime.FormatPretty()),
                Html.Tag(HtmlTextWriterTag.Td).Class("hidden").Content(testExecutionTime.Ticks.ToString(CultureInfo.InvariantCulture)),
                Html.Tag(HtmlTextWriterTag.Td).Content(testAverageExecutionTime.FormatPretty()),
                Html.Tag(HtmlTextWriterTag.Td).Class("hidden").Content(testAverageExecutionTime.Ticks.ToString(CultureInfo.InvariantCulture))
                );
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

        private static IHtmlNode GetSummaryTableHeaders(params Tuple<string, string, string>[] headers)
        {
            return Html.Tag(HtmlTextWriterTag.Thead).Content(
                Html.Tag(HtmlTextWriterTag.Tr).Content(headers.Select(header =>
                    Html.Tag(HtmlTextWriterTag.Th)
                        .Class(header.Item2)
                        .Content(header.Item1)
                        .OnClick(header.Item3))));
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

            foreach (var htmlNode in GetOptionsNodes())
                yield return htmlNode;

            for (var i = 0; i < features.Length; ++i)
                yield return GetFeatureDetails(features[i], i + 1);
        }

        private static IEnumerable<IHtmlNode> GetOptionsNodes()
        {
            return new[]
            {
                Html.Tag(HtmlTextWriterTag.Span).Class("options").Content("Filter:"),
                Html.Checkbox().Id("showPassed").Checked().SpaceBefore(),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Passed")).For("showPassed"),
                Html.Checkbox().Id("showBypassed").Checked().SpaceBefore(),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Bypassed")).For("showBypassed"),
                Html.Checkbox().Id("showFailed").Checked().SpaceBefore(),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Failed")).For("showFailed"),
                Html.Checkbox().Id("showIgnored").Checked().SpaceBefore(),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Ignored")).For("showIgnored"),
                Html.Checkbox().Id("showNotRun").Checked().SpaceBefore(),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Not Run")).For("showNotRun"),
                Html.Br()
            };
        }

        private static IEnumerable<IHtmlNode> GetFilterNodes()
        {
            return new[]
            {
                Html.Tag(HtmlTextWriterTag.Span).Class("options").Content("Toggle:"),
                Html.Checkbox().Id("toggleFeatures").Checked().SpaceBefore().OnClick("checkAll('toggleF',toggleFeatures.checked)"),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Features")).For("toggleFeatures"),
                Html.Checkbox().Id("toggleScenarios").Checked().SpaceBefore().OnClick("checkAll('toggleS',toggleScenarios.checked)"),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(),Html.Text("Scenarios")).For("toggleScenarios"),
                Html.Br()
            };
        }

        private static IHtmlNode GetCheckBoxTag()
        {
            return Html.Tag(HtmlTextWriterTag.Span).Class("chbox");
        }

        private static IHtmlNode GetFeatureDetails(IFeatureResult feature, int index)
        {
            return Html.Tag(Html5Tag.Article).Class(GetFeatureClasses(feature)).Content(
                Html.Checkbox().Class("toggle toggleF").Id("toggle" + index).Checked(),
                Html.Tag(HtmlTextWriterTag.Div).Class("header").Content(
                    Html.Tag(HtmlTextWriterTag.H2).Id("feature" + index).Class("title").Content(
                        Html.Tag(HtmlTextWriterTag.Label).For("toggle" + index).Content(GetCheckBoxTag(), Html.Text(feature.Name)),
                        GetLabel(feature.Label),
                        GetSmallLink("feature" + index)),
                    Html.Tag(HtmlTextWriterTag.Div).Class("description").Content(feature.Description)),
                Html.Tag(HtmlTextWriterTag.Div).Class("scenarios").Content(
                    feature.Scenarios.Select((s, i) => GetScenario(s, index, i))));
        }

        private static IHtmlNode GetSmallLink(string link)
        {
            return Html.Tag(HtmlTextWriterTag.A).Class("smallLink").Href("#" + link).Content("[&#8734;link]", false, false);
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

        private static IHtmlNode GetScenario(IScenarioResult scenario, int featureIndex, int scenarioIndex)
        {
            var toggleId = string.Format("toggle{0}_{1}", featureIndex, scenarioIndex);
            var scenarioId = string.Format("scenario{0}_{1}", featureIndex, scenarioIndex + 1);
            return Html.Tag(HtmlTextWriterTag.Div).Class("scenario " + GetStatusClass(scenario.Status)).Content(
                Html.Checkbox().Id(toggleId).Class("toggle toggleS").Checked(),
                Html.Tag(HtmlTextWriterTag.H3).Id(scenarioId).Class("title").Content(
                    Html.Tag(HtmlTextWriterTag.Label).For(toggleId).Content(
                        GetCheckBoxTag(),
                        GetStatus(scenario.Status),
                        Html.Text(scenario.Name)),
                    GetLabel(scenario.Label),
                    GetDuration(scenario.ExecutionTime),
                    GetSmallLink(scenarioId)),
                Html.Tag(HtmlTextWriterTag.Div).Content(scenario.Steps.Select(GetStep)),
                GetStatusDetails(scenario.StatusDetails),
                Html.Br());
        }

        private static TagBuilder GetStatusDetails(string statusDetails)
        {
            return Html.Tag(HtmlTextWriterTag.Div).Class("details").Content(statusDetails).SkipEmpty();
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
                Html.Text(string.Format("{0}. {1}", step.Number, step.StepName.Format(_stepNameDecorator))).Trim(),
                GetDuration(step.ExecutionTime),
                GetStatusDetails(step.StatusDetails));
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
                        Html.Tag(HtmlTextWriterTag.Style).Content(_styles, false, false),
                        Html.Tag(HtmlTextWriterTag.Script).Content(_scripts, false, false)),
                    Html.Tag(HtmlTextWriterTag.Body).Content(
                        WriteExecutionSummary(features),
                        WriteFeatureList(features),
                        WriteFeatureDetails(features)
                        )))
                .Flush();
        }
    }
}