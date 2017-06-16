using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Reporting.Formatters.Html
{
    internal class HtmlResultTextWriter : IDisposable
    {
        private static readonly IStepNameDecorator StepNameDecorator = new HtmlStepNameDecorator();
        private readonly HtmlTextWriter _writer;
        private readonly string _styles = ReadResource("LightBDD.Framework.Reporting.Formatters.Html.styles.css");
        private readonly string _scripts = ReadResource("LightBDD.Framework.Reporting.Formatters.Html.scripts.js");
        private readonly string _favico = ReadBase64Resource("LightBDD.Framework.Reporting.Formatters.Html.lightbdd_small.ico");

        private readonly IFeatureResult[] _features;

        private readonly IDictionary<string, string> _categories;

        public HtmlResultTextWriter(Stream outputStream, IFeatureResult[] features)
        {
            _writer = new HtmlTextWriter(new StreamWriter(outputStream));
            _features = features;
            _categories = GroupCategories(features);
        }

        private static Dictionary<string, string> GroupCategories(IEnumerable<IFeatureResult> features)
        {
            return features
                .SelectMany(f => f.GetScenarios())
                .SelectMany(s => s.Info.Categories)
                .Distinct()
                .Select((c, i) => new KeyValuePair<string, string>(c, string.Format("_{0}_", i)))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static string ReadResource(string path)
        {
            using (var stream = typeof(HtmlResultTextWriter).GetTypeInfo().Assembly.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private static string ReadBase64Resource(string path)
        {
            using (var stream = typeof(HtmlResultTextWriter).GetTypeInfo().Assembly.GetManifestResourceStream(path))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private IHtmlNode WriteExecutionSummary()
        {
            var bypassedScenarios = _features.CountScenariosWithStatus(ExecutionStatus.Bypassed);
            var failedScenarios = _features.CountScenariosWithStatus(ExecutionStatus.Failed);
            var ignoredScenarios = _features.CountScenariosWithStatus(ExecutionStatus.Ignored);
            var timeSummary = _features.GetTestExecutionTimeSummary();

            return Html.Tag(Html5Tag.Section).Content(
                Html.Tag(Html5Tag.H1).Content("Execution summary"),
                Html.Tag(Html5Tag.Article).Content(
                    Html.Tag(Html5Tag.Table).Class("summary").Content(
                        GetKeyValueTableRow("Test execution start time:", timeSummary.Start.ToString("yyyy-MM-dd HH:mm:ss UTC")),
                        GetKeyValueTableRow("Test execution end time:", timeSummary.End.ToString("yyyy-MM-dd HH:mm:ss UTC")),
                        GetKeyValueTableRow("Test execution time:", timeSummary.Duration.FormatPretty()),
                        GetKeyValueTableRow("Test execution time (aggregated):", timeSummary.Aggregated.FormatPretty()),
                        GetKeyValueTableRow("Number of features:", _features.Length.ToString()),
                        GetKeyValueTableRow("Number of scenarios:", _features.CountScenarios()),
                        GetKeyValueTableRow("Passed scenarios:", _features.CountScenariosWithStatus(ExecutionStatus.Passed)),
                        GetKeyValueTableRow("Bypassed scenarios:", bypassedScenarios, "bypassedAlert", "bypassedDetails"),
                        GetKeyValueTableRow("Failed scenarios:", failedScenarios, "failedAlert", "failedDetails"),
                        GetKeyValueTableRow("Ignored scenarios:", ignoredScenarios, "ignoredAlert", "ignoredDetails"),
                        GetKeyValueTableRow("Number of steps:", _features.CountSteps()),
                        GetKeyValueTableRow("Passed steps:", _features.CountStepsWithStatus(ExecutionStatus.Passed)),
                        GetKeyValueTableRow("Bypassed steps:", _features.CountStepsWithStatus(ExecutionStatus.Bypassed), "bypassedAlert"),
                        GetKeyValueTableRow("Failed steps:", _features.CountStepsWithStatus(ExecutionStatus.Failed), "failedAlert"),
                        GetKeyValueTableRow("Ignored steps:", _features.CountStepsWithStatus(ExecutionStatus.Ignored), "ignoredAlert"),
                        GetKeyValueTableRow("Not Run steps:", _features.CountStepsWithStatus(ExecutionStatus.NotRun)))));
        }

        private static IHtmlNode GetKeyValueTableRow(string key, string value)
        {
            return Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Th).Content(key),
                Html.Tag(Html5Tag.Td).Content(value));
        }

        private static IHtmlNode GetKeyValueTableRow(string key, int value, string classNameIfNotZero = null, string detailsId = null)
        {
            var valueTag = Html.Tag(Html5Tag.Span).Content(value.ToString());

            if (classNameIfNotZero != null && value != 0)
                valueTag.Class(classNameIfNotZero);

            var detailsTag = (detailsId != null && value != 0)
                ? Html.Tag(Html5Tag.A)
                    .Id(detailsId)
                    .Href("#")
                    .Content("(see details)")
                    .SpaceBefore()
                : Html.Nothing();

            return Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Th).Content(key),
                Html.Tag(Html5Tag.Td).Content(valueTag, detailsTag));
        }

        private IHtmlNode WriteFeatureList()
        {
            return Html.Tag(Html5Tag.Section).Content(
                Html.Tag(Html5Tag.H1).Content("Feature summary"),
                Html.Tag(Html5Tag.Article).Content(
                    Html.Tag(Html5Tag.Table).Id("featuresSummary").Class("features").Content(
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
                Tuple.Create("Aggregated", sortableMinor, "sortTable('featuresSummary',15,true,this)"),
                Tuple.Create("", hidden, ""),
                Tuple.Create("Average", sortableMinor, "sortTable('featuresSummary',17,true,this)"),
                Tuple.Create("", hidden, "")
                );
            yield return Html.Tag(Html5Tag.Tbody).Content(_features.Select((t, index) => GetFeatureSummary(t, index + 1)));

        }

        private static IHtmlNode GetFeatureSummary(IFeatureResult feature, int index)
        {
            var timeSummary = feature.GetScenarios().GetTestExecutionTimeSummary();

            return Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Td).Content(
                    Html.Tag(Html5Tag.A).Href("#feature" + index).Content(feature.Info.Name.Format(StepNameDecorator)),
                    Html.Tag(Html5Tag.Span).Content(feature.Info.Labels.Select(GetLabel)).SkipEmpty()),

                Html.Tag(Html5Tag.Td).Content(feature.GetScenarios().Count().ToString()),
                Html.Tag(Html5Tag.Td).Content(feature.CountScenariosWithStatus(ExecutionStatus.Passed).ToString()),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "bypassedAlert", feature.CountScenariosWithStatus(ExecutionStatus.Bypassed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "failedAlert", feature.CountScenariosWithStatus(ExecutionStatus.Failed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "ignoredAlert", feature.CountScenariosWithStatus(ExecutionStatus.Ignored)),

                Html.Tag(Html5Tag.Td).Content(feature.CountSteps().ToString()),
                Html.Tag(Html5Tag.Td).Content(feature.CountStepsWithStatus(ExecutionStatus.Passed).ToString()),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "bypassedAlert", feature.CountStepsWithStatus(ExecutionStatus.Bypassed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "failedAlert", feature.CountStepsWithStatus(ExecutionStatus.Failed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "ignoredAlert", feature.CountStepsWithStatus(ExecutionStatus.Ignored)),
                Html.Tag(Html5Tag.Td).Content(feature.CountStepsWithStatus(ExecutionStatus.NotRun).ToString()),

                Html.Tag(Html5Tag.Td).Content(timeSummary.Duration.FormatPretty()),
                Html.Tag(Html5Tag.Td).Class("hidden").Content(timeSummary.Duration.Ticks.ToString()),
                Html.Tag(Html5Tag.Td).Content(timeSummary.Aggregated.FormatPretty()),
                Html.Tag(Html5Tag.Td).Class("hidden").Content(timeSummary.Aggregated.Ticks.ToString()),
                Html.Tag(Html5Tag.Td).Content(timeSummary.Average.FormatPretty()),
                Html.Tag(Html5Tag.Td).Class("hidden").Content(timeSummary.Average.Ticks.ToString())
                );
        }

        private static IHtmlNode GetNumericTagWithOptionalClass(Html5Tag tag, string className, int value)
        {
            var node = Html.Tag(tag).Content(value.ToString());
            if (value != 0)
                node.Class(className);
            return node;
        }

        private static IHtmlNode GetLabel(string label)
        {
            return Html.Tag(Html5Tag.Span)
                .Class("label")
                .Content(string.IsNullOrWhiteSpace(label) ? string.Empty : string.Format("[{0}]", label.Trim()))
                .SpaceBefore()
                .SkipEmpty();
        }

        private static IHtmlNode GetSummaryTableHeaders(params Tuple<string, string, string>[] headers)
        {
            return Html.Tag(Html5Tag.Thead).Content(
                Html.Tag(Html5Tag.Tr).Content(headers.Select(header =>
                    Html.Tag(Html5Tag.Th)
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
            yield return Html.Tag(Html5Tag.H1).Id("featureDetails").Content(Html.Text("Feature details"), GetSmallLink("featureDetails"));
            yield return Html.Tag(Html5Tag.Div).Class("optionsPanel").Content(
                GetToggleNodes(),
                GetStatusFilterNodes(),
                GetCategoryFilterNodes(),
                Html.Tag(Html5Tag.A).Class("shareable").Href("").Content("[&#8734;filtered link]", false, false).Id("optionsLink"));

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

            return Html.Tag(Html5Tag.Div).Class("options").Content(
                    Html.Tag(Html5Tag.Span).Content("Categories:"),
                    Html.Tag(Html5Tag.Span).Content(categories));
        }

        private static IHtmlNode GetCategoryFilterNode(string categoryId, string categoryName, bool selected = false)
        {
            return GetOptionNode(
                string.Format("category{0}radio", categoryId),
                Html.Radio().Name("categoryFilter")
                    .Attribute("data-filter-value", categoryId)
                    .Attribute("data-filter-name", WebUtility.UrlEncode(categoryName))
                    .OnClick("applyFilter()")
                    .Checked(selected)
                    .SpaceBefore(),
                categoryName);
        }

        private static IHtmlNode GetStatusFilterNodes()
        {
            return Html.Tag(Html5Tag.Div).Class("options").Content(
                Html.Tag(Html5Tag.Span).Content("Filter:"),
                Html.Tag(Html5Tag.Span).Content(
                    GetOptionNode("showPassed", GetStatusFilter(ExecutionStatus.Passed), "Passed"),
                    GetOptionNode("showBypassed", GetStatusFilter(ExecutionStatus.Bypassed), "Bypassed"),
                    GetOptionNode("showFailed", GetStatusFilter(ExecutionStatus.Failed), "Failed"),
                    GetOptionNode("showIgnored", GetStatusFilter(ExecutionStatus.Ignored), "Ignored"),
                    GetOptionNode("showNotRun", GetStatusFilter(ExecutionStatus.NotRun), "Not Run")));
        }

        private static TagBuilder GetStatusFilter(ExecutionStatus value)
        {
            return Html.Checkbox().Name("statusFilter").Attribute("data-filter-value", value.ToString().ToLower()).Checked().OnClick("applyFilter()").SpaceBefore();
        }

        private static IHtmlNode GetToggleNodes()
        {
            return Html.Tag(Html5Tag.Div).Class("options").Content(
                Html.Tag(Html5Tag.Span).Content("Toggle:"),
                Html.Tag(Html5Tag.Span).Content(
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
            return Html.Tag(Html5Tag.Span).Class("option").Content(element.Id(elementId),
                Html.Tag(Html5Tag.Label).Content(GetCheckBoxTag(), Html.Text(labelContent)).For(elementId));
        }

        private static IHtmlNode GetCheckBoxTag()
        {
            return Html.Tag(Html5Tag.Span).Class("chbox");
        }

        private IHtmlNode GetFeatureDetails(IFeatureResult feature, int index)
        {
            return Html.Tag(Html5Tag.Article).Class(GetFeatureClasses(feature)).Content(
                Html.Checkbox().Class("toggle toggleF").Id("toggle" + index).Checked(),
                Html.Tag(Html5Tag.Div).Class("header").Content(
                    Html.Tag(Html5Tag.H2).Id("feature" + index).Class("title").Content(
                        Html.Tag(Html5Tag.Label).For("toggle" + index).Content(GetCheckBoxTag(), Html.Text(feature.Info.Name.Format(StepNameDecorator))),
                        Html.Tag(Html5Tag.Span).Content(feature.Info.Labels.Select(GetLabel)).SkipEmpty(),
                        GetSmallLink("feature" + index)),
                    Html.Tag(Html5Tag.Div).Class("description").Content(feature.Info.Description)),
                Html.Tag(Html5Tag.Div).Class("scenarios").Content(
                    feature.GetScenarios().Select((s, i) => GetScenario(s, index, i))));
        }

        private static TagBuilder GetSmallLink(string link)
        {
            return Html.Tag(Html5Tag.A).Class("smallLink shareable").Href("#" + link).Content("[&#8734;link]", false, false);
        }

        private static string GetFeatureClasses(IFeatureResult feature)
        {
            var builder = new StringBuilder("feature");
            foreach (var result in Enum.GetValues(typeof(ExecutionStatus)).Cast<ExecutionStatus>().Where(result => feature.CountScenariosWithStatus(result) > 0))
                builder.Append(" ").Append(GetStatusClass(result));

            if (!feature.GetScenarios().Any())
                builder.Append(" ").Append(GetStatusClass(ExecutionStatus.NotRun));

            return builder.ToString();
        }

        private IHtmlNode GetScenario(IScenarioResult scenario, int featureIndex, int scenarioIndex)
        {
            var toggleId = string.Format("toggle{0}_{1}", featureIndex, scenarioIndex);
            var scenarioId = string.Format("scenario{0}_{1}", featureIndex, scenarioIndex + 1);

            return Html.Tag(Html5Tag.Div).Class("scenario " + GetStatusClass(scenario.Status)).Attribute("data-categories", GetScenarioCategories(scenario)).Content(
                Html.Checkbox().Id(toggleId).Class("toggle toggleS").Checked(),
                Html.Tag(Html5Tag.H3).Id(scenarioId).Class("title").Content(
                    Html.Tag(Html5Tag.Label).For(toggleId).Content(
                        GetCheckBoxTag(),
                        GetStatus(scenario.Status),
                        Html.Text(scenario.Info.Name.Format(StepNameDecorator))),
                    Html.Tag(Html5Tag.Span).Content(scenario.Info.Labels.Select(GetLabel)).SkipEmpty(),
                    GetDuration(scenario.ExecutionTime),
                    GetSmallLink(scenarioId)),
                Html.Tag(Html5Tag.Div).Class("categories").Content(string.Join(", ", scenario.Info.Categories)).SkipEmpty(),
                Html.Tag(Html5Tag.Div).Content(scenario.GetSteps().Select(GetStep)),
                GetStatusDetails(scenario.StatusDetails),
                GetComments(scenario.GetAllSteps()),
                Html.Br());
        }

        private IHtmlNode GetComments(IEnumerable<IStepResult> steps)
        {
            return Html.Tag(Html5Tag.Div).Class("comments")
                .Content(from s in steps from c in s.Comments select Html.Tag(Html5Tag.Div).Content($"// Step {s.Info.GroupPrefix}{s.Info.Number}: {c}"));
        }

        private string GetScenarioCategories(IScenarioResult scenario)
        {
            return GetScenarioCategories(scenario.Info.Categories);
        }

        private string GetScenarioCategories(IEnumerable<string> categories)
        {
            return string.Join(" ", categories.Select(cat => _categories[cat]));
        }

        private static TagBuilder GetStatusDetails(string statusDetails)
        {
            return Html.Tag(Html5Tag.Div).Class("details").Content(statusDetails).SkipEmpty();
        }

        private static IHtmlNode GetDuration(ExecutionTime executionTime)
        {
            return Html.Tag(Html5Tag.Span)
                .Class("duration")
                .Content(executionTime != null ? string.Format("({0})", executionTime.Duration.FormatPretty()) : string.Empty)
                .SkipEmpty()
                .SpaceBefore();
        }

        private static IHtmlNode GetStatus(ExecutionStatus status)
        {
            return Html.Tag(Html5Tag.Span)
                .Class("status " + GetStatusClass(status))
                .Content(status.ToString())
                .SpaceAfter();
        }

        private static IHtmlNode GetStep(IStepResult step)
        {
            return Html.Tag(Html5Tag.Div).Class("step").Content(
                GetStatus(step.Status),
                Html.Text(string.Format("{0}{1}. {2}", step.Info.GroupPrefix, step.Info.Number, step.Info.Name.Format(StepNameDecorator))).Trim(),
                GetDuration(step.ExecutionTime),
                Html.Tag(Html5Tag.Div).Class("sub-steps").Content(step.GetSubSteps().Select(GetStep)).SkipEmpty());
        }

        private static string GetStatusClass(ExecutionStatus status)
        {
            return status.ToString().ToLowerInvariant();
        }

        public void Write()
        {
            _writer
                .WriteTag(Html.Text("<!DOCTYPE HTML>"))
                .WriteTag(Html.Tag(Html5Tag.Html).Content(
                    Html.Tag(Html5Tag.Head).Content(
                        Html.Tag(Html5Tag.Meta).Attribute(Html5Attribute.Charset, "UTF-8"),
                        Html.Tag(Html5Tag.Link)
                            .Attribute(Html5Attribute.Rel, "shortcut icon")
                            .Attribute(Html5Attribute.Type, "image/x-icon")
                            .Attribute(Html5Attribute.Href, "data:image/ico;base64," + _favico),
                        Html.Tag(Html5Tag.Title).Content("Summary"),
                        Html.Tag(Html5Tag.Style).Content(_styles, false, false),
                        Html.Tag(Html5Tag.Script).Content(_scripts, false, false)),
                    Html.Tag(Html5Tag.Body).Content(
                        WriteExecutionSummary(),
                        WriteFeatureList(),
                        WriteFeatureDetails(),
                        Html.Tag(Html5Tag.Div).Class("footer").Content(Html.Text("Generated with "), Html.Tag(Html5Tag.A).Content("LightBDD v" + GetLightBddVersion()).Href("https://github.com/LightBDD/LightBDD")),
                        Html.Tag(Html5Tag.Script).Content("initialize();", false, false)
                        )));
        }

        private static string GetLightBddVersion()
        {
            return typeof(IBddRunner).GetTypeInfo().Assembly.GetName().Version.ToString(4);
        }
    }
}