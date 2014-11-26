using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
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
        private readonly IFeatureResult[] _features;
        private readonly IDictionary<string, string> _categories;

        public HtmlResultTextWriter(Stream outputStream, IFeatureResult[] features)
        {
            _writer = new HtmlTextWriter(new StreamWriter(outputStream), "");
            _features = features;
            _categories = GroupCategories(features);
        }

        private static Dictionary<string, string> GroupCategories(IEnumerable<IFeatureResult> features)
        {
            return features
                .SelectMany(f => f.Scenarios)
                .SelectMany(s => s.Categories)
                .Distinct()
                .Select((c, i) => new KeyValuePair<string, string>(c, string.Format("_{0}_", i)))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
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

        private IHtmlNode WriteExecutionSummary()
        {
            return Html.Tag(Html5Tag.Section).Content(
                Html.Tag(HtmlTextWriterTag.H1).Content("Execution summary"),
                Html.Tag(Html5Tag.Article).Content(
                    Html.Tag(HtmlTextWriterTag.Table).Class("summary").Content(
                        GetKeyValueTableRow("Test execution start time:", _features.GetTestExecutionStartTime().ToString("yyyy-MM-dd HH:mm:ss UTC")),
                        GetKeyValueTableRow("Test execution time:", _features.GetTestExecutionTime().FormatPretty()),
                        GetKeyValueTableRow("Number of features:", _features.Length.ToString()),
                        GetKeyValueTableRow("Number of scenarios:", _features.CountScenarios()),
                        GetKeyValueTableRow("Passed scenarios:", _features.CountScenariosWithStatus(ResultStatus.Passed)),
                        GetKeyValueTableRow("Bypassed scenarios:", _features.CountScenariosWithStatus(ResultStatus.Bypassed)),
                        GetKeyValueTableRow("Failed scenarios:", _features.CountScenariosWithStatus(ResultStatus.Failed), "alert"),
                        GetKeyValueTableRow("Ignored scenarios:", _features.CountScenariosWithStatus(ResultStatus.Ignored)),
                        GetKeyValueTableRow("Number of steps:", _features.CountSteps()),
                        GetKeyValueTableRow("Passed steps:", _features.CountStepsWithStatus(ResultStatus.Passed)),
                        GetKeyValueTableRow("Bypassed steps:", _features.CountStepsWithStatus(ResultStatus.Bypassed)),
                        GetKeyValueTableRow("Failed steps:", _features.CountStepsWithStatus(ResultStatus.Failed), "alert"),
                        GetKeyValueTableRow("Ignored steps:", _features.CountStepsWithStatus(ResultStatus.Ignored)),
                        GetKeyValueTableRow("Not Run steps:", _features.CountStepsWithStatus(ResultStatus.NotRun))
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
            var valueTag = Html.Tag(HtmlTextWriterTag.Td).Content(value.ToString());

            if (classNameIfNotZero != null && value != 0)
                valueTag.Class(classNameIfNotZero);

            return Html.Tag(HtmlTextWriterTag.Tr).Content(
                Html.Tag(HtmlTextWriterTag.Th).Content(key),
                valueTag);
        }

        private IHtmlNode WriteFeatureList()
        {
            return Html.Tag(Html5Tag.Section).Content(
                Html.Tag(HtmlTextWriterTag.H1).Content("Feature summary"),
                Html.Tag(Html5Tag.Article).Content(
                    Html.Tag(HtmlTextWriterTag.Table).Id("featuresSummary").Class("features").Content(
                        GetSummaryTable())));
        }

        private IEnumerable<IHtmlNode> GetSummaryTable()
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
            yield return Html.Tag(HtmlTextWriterTag.Tbody).Content(_features.Select((t, index) => GetFeatureSummary(t, index + 1)));

        }

        private static IHtmlNode GetFeatureSummary(IFeatureResult feature, int index)
        {
            var testExecutionTime = feature.Scenarios.GetTestExecutionTime();
            var testAverageExecutionTime = feature.Scenarios.GetTestAverageExecutionTime();

            return Html.Tag(HtmlTextWriterTag.Tr).Content(
                Html.Tag(HtmlTextWriterTag.Td).Content(
                    Html.Tag(HtmlTextWriterTag.A).Href("#feature" + index).Content(feature.Name),
                    GetLabel(feature.Label)),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.Scenarios.Count().ToString()),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Passed).ToString()),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Bypassed).ToString()),
                GetNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountScenariosWithStatus(ResultStatus.Failed)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountScenariosWithStatus(ResultStatus.Ignored).ToString()),

                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountSteps().ToString()),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Passed).ToString()),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Bypassed).ToString()),
                GetNumericTagWithOptionalClass(HtmlTextWriterTag.Td, "alert", feature.CountStepsWithStatus(ResultStatus.Failed)),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.Ignored).ToString()),
                Html.Tag(HtmlTextWriterTag.Td).Content(feature.CountStepsWithStatus(ResultStatus.NotRun).ToString()),

                Html.Tag(HtmlTextWriterTag.Td).Content(testExecutionTime.FormatPretty()),
                Html.Tag(HtmlTextWriterTag.Td).Class("hidden").Content(testExecutionTime.Ticks.ToString()),
                Html.Tag(HtmlTextWriterTag.Td).Content(testAverageExecutionTime.FormatPretty()),
                Html.Tag(HtmlTextWriterTag.Td).Class("hidden").Content(testAverageExecutionTime.Ticks.ToString())
                );
        }

        private static IHtmlNode GetNumericTagWithOptionalClass(HtmlTextWriterTag tag, string className, int value)
        {
            var node = Html.Tag(tag).Content(value.ToString());
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

        private IHtmlNode WriteFeatureDetails()
        {
            return Html.Tag(Html5Tag.Section).Content(
                GetFeatureDetailsContent());
        }

        private IEnumerable<IHtmlNode> GetFeatureDetailsContent()
        {
            yield return Html.Tag(HtmlTextWriterTag.H1).Id("featureDetails").Content(Html.Text("Feature details"), GetSmallLink("featureDetails"));
            yield return Html.Tag(HtmlTextWriterTag.Div).Class("optionsPanel").Content(
                GetToggleNodes(),
                GetStatusFilterNodes(),
                GetCategoryFilterNodes(),
                Html.Tag(HtmlTextWriterTag.A).Class("shareable").Href("").Content("[&#8734;filtered link]", false, false).Id("optionsLink"));

            for (var i = 0; i < _features.Length; ++i)
                yield return GetFeatureDetails(_features[i], i + 1);
        }

        private IHtmlNode GetCategoryFilterNodes()
        {
            if (_categories.Count == 0)
                return Html.Nothing();

            var categories = Enumerable.Repeat(GetCategoryFilterNode("all", "-all-", true), 1)
                .Concat(_categories.OrderBy(cat => cat.Key).Select(cat => GetCategoryFilterNode(cat.Value, cat.Key)))
                .Concat(Enumerable.Repeat(GetCategoryFilterNode("without", "-without category-"), 1));

            return Html.Tag(HtmlTextWriterTag.Div).Class("options").Content(
                    Html.Tag(HtmlTextWriterTag.Span).Content("Categories:"),
                    Html.Tag(HtmlTextWriterTag.Span).Content(categories));
        }

        private static IHtmlNode GetCategoryFilterNode(string categoryId, string categoryName, bool selected = false)
        {
            return GetOptionNode(
                string.Format("category{0}radio", categoryId),
                Html.Radio().Name("categoryFilter")
                    .Attribute("data-filter-value", categoryId)
                    .Attribute("data-filter-name", HttpUtility.UrlEncode(categoryName))
                    .OnClick("applyFilter()")
                    .Checked(selected)
                    .SpaceBefore(),
                categoryName);
        }

        private static IHtmlNode GetStatusFilterNodes()
        {
            return Html.Tag(HtmlTextWriterTag.Div).Class("options").Content(
                Html.Tag(HtmlTextWriterTag.Span).Content("Filter:"),
                Html.Tag(HtmlTextWriterTag.Span).Content(
                    GetOptionNode("showPassed", GetStatusFilter(ResultStatus.Passed), "Passed"),
                    GetOptionNode("showBypassed", GetStatusFilter(ResultStatus.Bypassed), "Bypassed"),
                    GetOptionNode("showFailed", GetStatusFilter(ResultStatus.Failed), "Failed"),
                    GetOptionNode("showIgnored", GetStatusFilter(ResultStatus.Ignored), "Ignored"),
                    GetOptionNode("showNotRun", GetStatusFilter(ResultStatus.NotRun), "Not Run")));
        }

        private static TagBuilder GetStatusFilter(ResultStatus value)
        {
            return Html.Checkbox().Name("statusFilter").Attribute("data-filter-value", value.ToString().ToLower()).Checked().OnClick("applyFilter()").SpaceBefore();
        }

        private static IHtmlNode GetToggleNodes()
        {
            return Html.Tag(HtmlTextWriterTag.Div).Class("options").Content(
                Html.Tag(HtmlTextWriterTag.Span).Content("Toggle:"),
                Html.Tag(HtmlTextWriterTag.Span).Content(
                GetOptionNode(
                    "toggleFeatures",
                    Html.Checkbox().Checked().SpaceBefore().OnClick("checkAll('toggleF',toggleFeatures.checked)"),
                    "Features"),

                GetOptionNode(
                    "toggleScenarios",
                    Html.Checkbox().Checked().SpaceBefore().OnClick("checkAll('toggleS',toggleScenarios.checked)"),
                    "Scenarios")));
        }

        private static IHtmlNode GetOptionNode(string elementId, TagBuilder element, string labelContent)
        {
            return Html.Tag(HtmlTextWriterTag.Span).Class("option").Content(element.Id(elementId),
                Html.Tag(HtmlTextWriterTag.Label).Content(GetCheckBoxTag(), Html.Text(labelContent)).For(elementId));
        }

        private static IHtmlNode GetCheckBoxTag()
        {
            return Html.Tag(HtmlTextWriterTag.Span).Class("chbox");
        }

        private IHtmlNode GetFeatureDetails(IFeatureResult feature, int index)
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

        private static TagBuilder GetSmallLink(string link)
        {
            return Html.Tag(HtmlTextWriterTag.A).Class("smallLink shareable").Href("#" + link).Content("[&#8734;link]", false, false);
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

        private IHtmlNode GetScenario(IScenarioResult scenario, int featureIndex, int scenarioIndex)
        {
            var toggleId = string.Format("toggle{0}_{1}", featureIndex, scenarioIndex);
            var scenarioId = string.Format("scenario{0}_{1}", featureIndex, scenarioIndex + 1);
            return Html.Tag(HtmlTextWriterTag.Div).Class("scenario " + GetStatusClass(scenario.Status)).Attribute("data-categories", GetScenarioCategories(scenario)).Content(
                Html.Checkbox().Id(toggleId).Class("toggle toggleS").Checked(),
                Html.Tag(HtmlTextWriterTag.H3).Id(scenarioId).Class("title").Content(
                    Html.Tag(HtmlTextWriterTag.Label).For(toggleId).Content(
                        GetCheckBoxTag(),
                        GetStatus(scenario.Status),
                        Html.Text(scenario.Name)),
                    GetLabel(scenario.Label),
                    GetDuration(scenario.ExecutionTime),
                    GetSmallLink(scenarioId)),
                Html.Tag(HtmlTextWriterTag.Div).Class("categories").Content(string.Join(", ", scenario.Categories)).SkipEmpty(),
                Html.Tag(HtmlTextWriterTag.Div).Content(scenario.Steps.Select(GetStep)),
                GetStatusDetails(scenario.StatusDetails),
                Html.Br());
        }

        private string GetScenarioCategories(IScenarioResult scenario)
        {
            return GetScenarioCategories(scenario.Categories);
        }

        private string GetScenarioCategories(IEnumerable<string> categories)
        {
            return string.Join(" ", categories.Select(cat => _categories[cat]));
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
                GetDuration(step.ExecutionTime));
        }

        private static string GetStatusClass(ResultStatus status)
        {
            return status.ToString().ToLowerInvariant();
        }

        public void Write()
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
                        WriteExecutionSummary(),
                        WriteFeatureList(),
                        WriteFeatureDetails(),
                        Html.Tag(HtmlTextWriterTag.Script).Content("applyOptionsFromLink();", false, false)
                        )))
                .Flush();
        }
    }
}