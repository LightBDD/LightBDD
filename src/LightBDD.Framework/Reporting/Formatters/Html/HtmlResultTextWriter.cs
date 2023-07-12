using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Reporting.Formatters.Html
{
    internal class HtmlResultTextWriter : IDisposable
    {
        private static readonly IStepNameDecorator StepNameDecorator = new HtmlStepNameDecorator();
        private readonly HtmlTextWriter _writer;
        private readonly string _styles = ReadResource("LightBDD.Framework.Reporting.Formatters.Html.Resources.styles.css");
        private readonly string _scripts = ReadResource("LightBDD.Framework.Reporting.Formatters.Html.Resources.scripts.js");
        private readonly string _favico = ReadBase64Resource("LightBDD.Framework.Reporting.Formatters.Html.Resources.lightbdd_small.ico");

        private readonly ITestRunResult _result;

        private readonly IDictionary<string, string> _categories;

        public HtmlResultTextWriter(Stream outputStream, ITestRunResult result)
        {
            _writer = new HtmlTextWriter(new StreamWriter(outputStream));
            _result = result;
            _categories = GroupCategories();
        }

        public void Write(HtmlReportFormatterOptions options)
        {
            _writer
                .WriteTag(Html.Text("<!DOCTYPE HTML>"))
                .WriteTag(Html.Tag(Html5Tag.Html).Attribute("lang","en").Content(
                    Html.Tag(Html5Tag.Head).Content(
                        Html.Tag(Html5Tag.Meta).Attribute(Html5Attribute.Charset, "UTF-8"),
                        Html.Tag(Html5Tag.Meta).Attribute(Html5Attribute.Name,"viewport").Attribute(Html5Attribute.Content, "width=device-width, initial-scale=1"),
                        GetFavicon(options.CustomFavicon),
                        Html.Tag(Html5Tag.Title).Content("Summary"),
                        Html.Tag(Html5Tag.Style).Content(EmbedCssImages(options), false, false),
                        Html.Tag(Html5Tag.Style).Content(_styles, false, false),
                        Html.Tag(Html5Tag.Style).Content(options.CssContent, false, false).SkipEmpty(),
                        Html.Tag(Html5Tag.Script).Content(_scripts, false, false)),
                    Html.Tag(Html5Tag.Body).Content(
                        WriteExecutionSummary(),
                        WriteFeatureSummary(),
                        WriteFeatureDetails(),
                        Html.Tag(Html5Tag.Div).Class("footer").Content(Html.Text("Generated with "), Html.Tag(Html5Tag.A).Content("LightBDD v" + GetLightBddVersion()).Href("https://github.com/LightBDD/LightBDD")),
                        Html.Tag(Html5Tag.Script).Content("initialize();", false, false)
                    )));
        }

        private TagBuilder GetFavicon(Tuple<string, byte[]> custom)
        {
            var type = "image/x-icon";
            var favicon = _favico;
            if (custom != null)
            {
                type = custom.Item1;
                favicon = Convert.ToBase64String(custom.Item2);
            }

            return Html.Tag(Html5Tag.Link)
                .Attribute(Html5Attribute.Rel, "icon")
                .Attribute(Html5Attribute.Type, type)
                .Attribute(Html5Attribute.Href, $"data:{type};base64,{favicon}");
        }

        private Dictionary<string, string> GroupCategories()
        {
            return _result.Features
                .SelectMany(f => f.GetScenarios())
                .SelectMany(s => s.Info.Categories)
                .Distinct()
                .Select((c, i) => new KeyValuePair<string, string>(c, $"_{i}_"))
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
            var bypassedScenarios = _result.Features.CountScenariosWithStatus(ExecutionStatus.Bypassed);
            var failedScenarios = _result.Features.CountScenariosWithStatus(ExecutionStatus.Failed);
            var ignoredScenarios = _result.Features.CountScenariosWithStatus(ExecutionStatus.Ignored);

            return Html.Tag(Html5Tag.Section).Class("execution-summary").Content(
                Html.Tag(Html5Tag.H1).Content("Test execution summary"),
                Html.Tag(Html5Tag.Div).Class("content").Content(
                    Html.Tag(Html5Tag.Table).Content(
                        GetKeyValueHeaderTableRow("Execution"),
                        GetKeyValueTableRow("Project:", _result.Info.TestSuite.Name),
                        GetOverallStatus(),
                        GetKeyValueTableRow("Start date:", _result.ExecutionTime.Start.ToString("yyyy-MM-dd (UTC)")),
                        GetKeyValueTableRow("Start time:", _result.ExecutionTime.Start.ToString("HH:mm:ss")),
                        GetKeyValueTableRow("End time:", _result.ExecutionTime.End.ToString("HH:mm:ss")),
                        GetKeyValueTableRow("Duration:", _result.ExecutionTime.Duration.FormatPretty())),
                    Html.Tag(Html5Tag.Table).Content(
                        GetKeyValueHeaderTableRow("Content"),
                        GetKeyValueTableRow("Features:", _result.Features.Count.ToString()),
                        GetKeyValueTableRow("Scenarios:", _result.Features.CountScenarios()),
                        GetKeyValueTableRow("Passed scenarios:", _result.Features.CountScenariosWithStatus(ExecutionStatus.Passed)),
                        GetKeyValueTableRow("Bypassed scenarios:", bypassedScenarios, "bypassedAlert", "bypassedDetails", true),
                        GetKeyValueTableRow("Failed scenarios:", failedScenarios, "failedAlert", "failedDetails", true),
                        GetKeyValueTableRow("Ignored scenarios:", ignoredScenarios, "ignoredAlert", "ignoredDetails", true))));
        }

        private TagBuilder GetOverallStatus()
        {
            var executionStatus = _result.Features.SelectMany(f => f.GetScenarios()).Select(s => s.Status).OrderByDescending(x => x).DefaultIfEmpty(ExecutionStatus.NotRun).First();
            if (executionStatus != ExecutionStatus.Failed)
                executionStatus = ExecutionStatus.Passed;
            return Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Th).Content("Overall status:"),
                Html.Tag(Html5Tag.Td).Class($"overall-status {GetStatusClass(executionStatus)}").Content(executionStatus.ToString()));
        }

        private static IHtmlNode GetKeyValueTableRow(string key, string value)
        {
            return Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Th).Content(key),
                Html.Tag(Html5Tag.Td).Content(value));
        }

        private static IHtmlNode GetKeyValueHeaderTableRow(string key)
        {
            return Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Th).Content(key).Class("subHeader").Attribute(Html5Attribute.Colspan, "2"));
        }

        private static IHtmlNode GetKeyValueTableRow(string key, int value, string classNameIfNotZero = null, string detailsId = null, bool ignoreIfZero = false)
        {
            if (ignoreIfZero && value == 0)
                return Html.Nothing();

            var valueTag = Html.Tag(Html5Tag.Span).Content(value.ToString());

            if (classNameIfNotZero != null && value != 0)
                valueTag.Class(classNameIfNotZero);

            var detailsTag = detailsId != null && value != 0
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

        private IHtmlNode WriteFeatureSummary()
        {
            return Html.Tag(Html5Tag.Section).Class("features-summary").Content(
                Html.Tag(Html5Tag.H1).Content("Feature summary"),
                Html.Tag(Html5Tag.Div).Class("content").Content(
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
            yield return Html.Tag(Html5Tag.Tbody).Content(_result.Features.Select((t, index) => GetFeatureSummary(t, index + 1)));

            yield return GetFeaturesSummaryFooter();
        }

        private IHtmlNode GetFeaturesSummaryFooter()
        {
            var timeSummary = _result.Features.GetTestExecutionTimeSummary();
            return Html.Tag(Html5Tag.Tfoot).Content(Html.Tag(Html5Tag.Tr).Content(
                Html.Tag(Html5Tag.Td).Content("Totals"),

                Html.Tag(Html5Tag.Td).Content(_result.Features.CountScenarios().ToString()),
                Html.Tag(Html5Tag.Td).Content(_result.Features.CountScenariosWithStatus(ExecutionStatus.Passed).ToString()),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "bypassedAlert", _result.Features.CountScenariosWithStatus(ExecutionStatus.Bypassed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "failedAlert", _result.Features.CountScenariosWithStatus(ExecutionStatus.Failed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "ignoredAlert", _result.Features.CountScenariosWithStatus(ExecutionStatus.Ignored)),

                Html.Tag(Html5Tag.Td).Content(_result.Features.CountSteps().ToString()),
                Html.Tag(Html5Tag.Td).Content(_result.Features.CountStepsWithStatus(ExecutionStatus.Passed).ToString()),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "bypassedAlert", _result.Features.CountStepsWithStatus(ExecutionStatus.Bypassed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "failedAlert", _result.Features.CountStepsWithStatus(ExecutionStatus.Failed)),
                GetNumericTagWithOptionalClass(Html5Tag.Td, "ignoredAlert", _result.Features.CountStepsWithStatus(ExecutionStatus.Ignored)),
                Html.Tag(Html5Tag.Td).Content(_result.Features.CountStepsWithStatus(ExecutionStatus.NotRun).ToString()),

                Html.Tag(Html5Tag.Td).Content(timeSummary.Duration.FormatPretty()),
                Html.Tag(Html5Tag.Td).Class("hidden").Content(timeSummary.Duration.Ticks.ToString()),
                Html.Tag(Html5Tag.Td).Content(timeSummary.Aggregated.FormatPretty()),
                Html.Tag(Html5Tag.Td).Class("hidden").Content(timeSummary.Aggregated.Ticks.ToString()),
                Html.Tag(Html5Tag.Td).Content(timeSummary.Average.FormatPretty()),
                Html.Tag(Html5Tag.Td).Class("hidden").Content(timeSummary.Average.Ticks.ToString())
            ));
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
                .Content(string.IsNullOrWhiteSpace(label) ? string.Empty : label.Trim())
                .SpaceBefore()
                .SkipEmpty();
        }

        private static IHtmlNode GetCategory(string category)
        {
            return Html.Tag(Html5Tag.Span)
                .Class("category")
                .Content(string.IsNullOrWhiteSpace(category) ? string.Empty : category.Trim())
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
            return Html.Tag(Html5Tag.Section).Class("features").Content(
                GetFeatureDetailsContent());
        }

        private IEnumerable<IHtmlNode> GetFeatureDetailsContent()
        {
            yield return Html.Tag(Html5Tag.H1).Id("featureDetails").Content(Html.Text("Feature details"), GetSmallLink("featureDetails"));
            yield return Html.Tag(Html5Tag.Div).Class("optionsPanel").Content(
                GetToggleNodes(),
                GetStatusFilterNodes(),
                GetCategoryFilterNodes(),
                Html.Tag(Html5Tag.A).Class("shareable").Href("").Content("filtered link", false, false).Id("optionsLink").SpaceBefore());

            for (var i = 0; i < _result.Features.Count; ++i)
                yield return GetFeatureDetails(_result.Features[i], i + 1);
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
                $"category{categoryId}radio",
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
                        "Scenarios"),
                    GetOptionNode(
                        "toggleSubSteps",
                        Html.Checkbox().Checked().SpaceBefore().OnClick("checkAll('toggleSS',toggleSubSteps.checked)"),
                        "Sub Steps")));
        }

        private static IHtmlNode GetOptionNode(string elementId, TagBuilder element, string labelContent)
        {
            return Html.Tag(Html5Tag.Span).Class("option").Content(element.Id(elementId),
                Html.Tag(Html5Tag.Label).Content(GetCheckBoxTag(), Html.Text(labelContent)).For(elementId));
        }

        private static IHtmlNode GetCheckBoxTag(bool isEmpty = false)
        {
            var className = isEmpty ? "chbox empty" : "chbox";
            return Html.Tag(Html5Tag.Span).Class(className);
        }

        private IHtmlNode GetFeatureDetails(IFeatureResult feature, int index)
        {
            return Html.Tag(Html5Tag.Article).Class(GetFeatureClasses(feature)).Content(
                Html.Checkbox().Class("toggle toggleF").Id("toggle" + index).Checked(),
                Html.Tag(Html5Tag.H2).Id("feature" + index).Class("title header").Content(
                    Html.Tag(Html5Tag.Label).For("toggle" + index).Class("controls").Content(GetCheckBoxTag()),
                    Html.Tag(Html5Tag.Span).Class("content").Content(
                        Html.Text(feature.Info.Name.Format(StepNameDecorator)),
                        Html.Tag(Html5Tag.Span).Content(feature.Info.Labels.Select(GetLabel)).SkipEmpty(),
                        GetSmallLink("feature" + index))),
                Html.Tag(Html5Tag.Div).Class("description").Content(feature.Info.Description),
                Html.Tag(Html5Tag.Div).Class("scenarios").Content(
                    feature.GetScenariosOrderedByName().Select((s, i) => GetScenario(s, index, i))));
        }

        private static TagBuilder GetSmallLink(string link)
        {
            return Html.Tag(Html5Tag.A).Class("smallLink shareable").Href("#" + link).Content("link", false, false).SpaceBefore();
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
            var toggleId = $"toggle{featureIndex}_{scenarioIndex}";
            var scenarioId = $"scenario{featureIndex}_{scenarioIndex + 1}";

            return Html.Tag(Html5Tag.Div).Class("scenario " + GetStatusClass(scenario.Status))
                .Attribute("data-categories", GetScenarioCategories(scenario))
                .Content(
                    Html.Checkbox().Id(toggleId).Class("toggle toggleS").Checked(),
                    Html.Tag(Html5Tag.H3).Id(scenarioId).Class("header title").Content(
                        Html.Tag(Html5Tag.Label).For(toggleId).Class("controls").Content(
                            GetCheckBoxTag(),
                            GetStatus(scenario.Status)),
                        Html.Tag(Html5Tag.Span).Content(
                            Html.Text(scenario.Info.Name.Format(StepNameDecorator)),
                            Html.Tag(Html5Tag.Span).Content(scenario.Info.Labels.Select(GetLabel)).SkipEmpty(),
                            GetDuration(scenario.ExecutionTime),
                            GetSmallLink(scenarioId))),
                    Html.Tag(Html5Tag.Div).Class("content").Content(
                        Html.Tag(Html5Tag.Div).Class("categories")
                            .Content(scenario.Info.Categories.Select(GetCategory))
                            .SkipEmpty(),
                        Html.Tag(Html5Tag.Div).Class("scenario-steps").Content(scenario.GetSteps().Select(GetStep))),
                    Html.Tag(Html5Tag.Div).Class("details").Content(
                        GetStatusDetails(scenario.StatusDetails),
                        GetComments(scenario.GetAllSteps()),
                        GetAttachments(scenario.GetAllSteps())).SkipEmpty());
        }

        private static IHtmlNode GetScenarioDetailsSection(string className, string description, IEnumerable<IHtmlNode> content)
        {
            var nodes = content.ToArray();
            if (nodes.All(n => n.IsEmpty()))
                return Html.Nothing();
            return Html.Tag(Html5Tag.Div).Class(className).Content(Enumerable.Repeat(Html.Tag(Html5Tag.H3).Content(description), 1).Concat(nodes));
        }
        private IHtmlNode GetAttachments(IEnumerable<IStepResult> steps)
        {
            return GetScenarioDetailsSection("attachments", "Attachments:",
                from s in steps
                from a in s.FileAttachments
                select
                    Html.Tag(Html5Tag.Div).Content(
                        Html.Tag(Html5Tag.A)
                            .Href(ResolveLink(a))
                            .Attribute("target", "_blank")
                            .Content(Html.Tag(Html5Tag.Code).Content(
                                $"🔗Step {s.Info.GroupPrefix}{s.Info.Number}: {a.Name} ({Path.GetExtension(a.FilePath).TrimStart('.')})"))));
        }

        private string ResolveLink(FileAttachment fileAttachment)
        {
            return fileAttachment.RelativePath.Replace(Path.DirectorySeparatorChar, '/');
        }

        private IHtmlNode GetComments(IEnumerable<IStepResult> steps)
        {
            return GetScenarioDetailsSection("comments", "Comments:",
                from s in steps
                from c in s.Comments
                select Html.Tag(Html5Tag.Div).Content(Html.Tag(Html5Tag.Code).Content($"// Step {s.Info.GroupPrefix}{s.Info.Number}: {c}")));
        }

        private string GetScenarioCategories(IScenarioResult scenario)
        {
            return GetScenarioCategories(scenario.Info.Categories);
        }

        private string GetScenarioCategories(IEnumerable<string> categories)
        {
            return string.Join(" ", categories.Select(cat => _categories[cat]));
        }

        private static IHtmlNode GetStatusDetails(string statusDetails)
        {
            return GetScenarioDetailsSection("status-details", "Details:", new[] { Html.Tag(Html5Tag.Code).Content(statusDetails).SkipEmpty() });
        }

        private static IHtmlNode GetDuration(ExecutionTime executionTime)
        {
            return Html.Tag(Html5Tag.Span)
                .Class("duration")
                .Content(executionTime != null ? $"({executionTime.Duration.FormatPretty()})" : string.Empty)
                .SkipEmpty()
                .SpaceBefore();
        }

        private static IHtmlNode GetStatus(ExecutionStatus status)
        {
            return Html.Tag(Html5Tag.Span)
                .Class("status " + GetStatusClass(status))
                .Content(GetStatusValue(status))
                .SpaceAfter();
        }

        private static IHtmlNode GetStep(IStepResult step)
        {
            var toggleId = step.Info.RuntimeId.ToString();
            var hasSubSteps = step.GetSubSteps().Any();

            var checkbox = hasSubSteps
                ? Html.Checkbox().Id(toggleId).Class("toggle toggleSS").Checked()
                : Html.Nothing();

            var container = hasSubSteps
                ? Html.Tag(Html5Tag.Label).For(toggleId)
                : Html.Tag(Html5Tag.Span);

            return Html.Tag(Html5Tag.Div).Class("step").Content(
                checkbox,
                Html.Tag(Html5Tag.Div).Class("header").Content(
                    container.Class("controls").Content(
                        GetCheckBoxTag(!hasSubSteps),
                        GetStatus(step.Status)),
                    Html.Tag(Html5Tag.Span).Content(
                        Html.Text($"{WebUtility.HtmlEncode(step.Info.GroupPrefix)}{step.Info.Number}. {step.Info.Name.Format(StepNameDecorator)}").Trim(),
                        GetDuration(step.ExecutionTime))),
                Html.Tag(Html5Tag.Div).Class("step-parameters")
                    .Content(step.Parameters.Select(GetStepParameter))
                    .SkipEmpty(),
                Html.Tag(Html5Tag.Div).Class("sub-steps").Content(step.GetSubSteps().Select(GetStep))
                    .SkipEmpty());
        }

        private static IHtmlNode GetStepParameter(IParameterResult parameter)
        {
            if (parameter.Details is ITabularParameterDetails table)
                return GetTabularParameter(parameter.Name, table);
            if (parameter.Details is ITreeParameterDetails tree)
                return GetTreeParameter(parameter.Name, tree);
            return Html.Nothing();
        }

        private static IHtmlNode GetTreeParameter(string parameterName, ITreeParameterDetails tree)
        {
            return Html.Tag(Html5Tag.Div).Class("param").Content(
                Html.Tag(Html5Tag.Div).Content($"{parameterName}:"),
                Html.Tag(Html5Tag.Div).Class("param tree").Content(GetTreeNode(tree.Root)));
        }

        private static IHtmlNode GetTreeNode(ITreeParameterNodeResult node)
        {
            var type = node.Children.Any() ? "branch" : "leaf";
            return Html.Tag(Html5Tag.Div).Class($"tree node {type}").Content(
                Html.Tag(Html5Tag.Div).Class("detail").Content(
                    Html.Tag(Html5Tag.Span).Class("param node").Content(node.Node),
                    GetParamValue(node, Html5Tag.Div)),
                Html.Tag(Html5Tag.Div).Class("branches").Content(
                    node.Children.Where(ch => !ch.Children.Any())
                        .Concat(node.Children.Where(ch => ch.Children.Any()))
                        .Select(GetTreeNode)
                    ).SkipEmpty()
            );
        }

        private static IHtmlNode GetTabularParameter(string parameterName, ITabularParameterDetails table)
        {
            return Html.Tag(Html5Tag.Div).Class("param").Content(
                Html.Tag(Html5Tag.Div).Content($"{parameterName}:"),
                Html.Tag(Html5Tag.Table).Class("param table")
                    .Content(GetParameterTable(table)));
        }

        private static IEnumerable<IHtmlNode> GetParameterTable(ITabularParameterDetails table)
        {
            var columns = table.Columns.Select(col => Html.Tag(Html5Tag.Th).Class(col.IsKey ? "param column key" : "param column value").Content(col.Name)).ToList();
            var renderRowStatus = table.VerificationStatus != ParameterVerificationStatus.NotApplicable;

            if (renderRowStatus)
                columns.Insert(0, Html.Tag(Html5Tag.Th).Class("param column").Content("#"));

            yield return Html.Tag(Html5Tag.Thead)
                .Content(Html.Tag(Html5Tag.Tr)
                    .Content(columns));

            yield return Html.Tag(Html5Tag.Tbody).Content(table.Rows.Select(row => GetParameterTableRow(row, renderRowStatus)));
        }

        private static IHtmlNode GetParameterTableRow(ITabularParameterRow row, bool renderRowStatus)
        {
            var values = row.Values.Select(v => GetParamValue(v, Html5Tag.Td)).ToList();
            if (renderRowStatus)
                values.Insert(0, Html.Tag(Html5Tag.Td).Class("param type").Content(GetRowTypeContent(row)));
            return Html.Tag(Html5Tag.Tr).Content(values);
        }

        private static string GetRowTypeContent(ITabularParameterRow row)
        {
            if (row.Type == TableRowType.Surplus)
                return "+";
            if (row.Type == TableRowType.Missing)
                return "-";
            if (row.VerificationStatus == ParameterVerificationStatus.Success)
                return "=";
            if (row.VerificationStatus == ParameterVerificationStatus.NotApplicable)
                return " ";
            return "!";
        }

        private static IHtmlNode GetParamValue(IValueResult value, Html5Tag tag)
        {
            var element = Html.Tag(tag).Class("param value " + value.VerificationStatus.ToString().ToLowerInvariant());
            if (value.VerificationStatus == ParameterVerificationStatus.NotApplicable ||
                value.VerificationStatus == ParameterVerificationStatus.Success)
                return element.Content(value.Value);

            return element.Content(
                Html.Text(value.Value).Escape(),
                Html.Tag(Html5Tag.Hr),
                Html.Tag(Html5Tag.Span).Class("expected").Content(value.Expectation));
        }

        private static string GetStatusClass(ExecutionStatus status)
        {
            return status.ToString().ToLowerInvariant();
        }

        private static string GetStatusValue(ExecutionStatus status)
        {
            return status switch
            {
                ExecutionStatus.NotRun => "?",
                ExecutionStatus.Passed => "✓",
                ExecutionStatus.Bypassed => "~",
                ExecutionStatus.Ignored => "!",
                ExecutionStatus.Failed => "✕",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }

        private string EmbedCssImages(HtmlReportFormatterOptions options)
        {
            var sb = new StringBuilder();

            void EmbedImage(string varName, string mimeType, string base64Body)
            {
                sb.AppendLine($"{varName}: url('data:{mimeType};base64,{base64Body}');");
            }
            void EmbedSvg(string varName, string resourcePath)
            {
                EmbedImage(varName, "image/svg+xml", ReadBase64Resource(resourcePath));
            }

            sb.AppendLine("html {");

            var customLogo = options.CustomLogo;
            if (customLogo != null)
                EmbedImage("--logo-ico", customLogo.Item1, Convert.ToBase64String(customLogo.Item2));
            else
                EmbedSvg("--logo-ico", "LightBDD.Framework.Reporting.Formatters.Html.Resources.lightbdd_opt.svg");

            sb.Append("}");
            return sb.ToString();
        }

        private string GetLightBddVersion()
        {
            return typeof(IBddRunner).GetTypeInfo().Assembly.GetName().Version.ToString();
        }
    }
}