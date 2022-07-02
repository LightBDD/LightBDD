using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.AcceptanceTests.Helpers.Builders;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.Framework.Resources;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
#pragma warning disable 1998

namespace LightBDD.AcceptanceTests.Features
{
    public class HtmlReportContext
    {
        private readonly ResourceHandle<ChromeDriver> _driverHandle;
        private static string BaseDirectory => AppContext.BaseDirectory;
        private State<ChromeDriver> _driver;
        private State<IFeatureResult[]> _features;

        private string HtmlFileName { get; }
        private ChromeDriver Driver => _driver.GetValue(nameof(Driver));
        private ResultBuilder ResultBuilder { get; }
        private IFeatureResult[] Features => _features.GetValue(nameof(Features));

        public HtmlReportContext(ResourceHandle<ChromeDriver> driverHandle)
        {
            _driverHandle = driverHandle;
            HtmlFileName = Path.GetFullPath(BaseDirectory + Path.DirectorySeparatorChar + Guid.NewGuid() + ".html");
            ResultBuilder = new ResultBuilder();
        }

        public async Task Given_a_various_features_with_scenarios_and_categories()
        {
            var feature = ResultBuilder.NewFeature("featureA");
            feature.NewScenario(ExecutionStatus.Passed).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Bypassed).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Ignored).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Failed).WithCategories("catA", "catB");
            ResultBuilder.NewFeature("featureB").NewScenario(ExecutionStatus.Passed);
            ResultBuilder.NewFeature("featureC").NewScenario(ExecutionStatus.Bypassed);
            ResultBuilder.NewFeature("featureD").NewScenario(ExecutionStatus.Ignored);
            ResultBuilder.NewFeature("featureE").NewScenario(ExecutionStatus.Failed);
        }

        public async Task Given_a_various_features_with_scenarios_but_no_categories()
        {
            var feature = ResultBuilder.NewFeature("featureA");
            feature.NewScenario(ExecutionStatus.Passed);
            feature.NewScenario(ExecutionStatus.Bypassed);
            feature.NewScenario(ExecutionStatus.Ignored);
            feature.NewScenario(ExecutionStatus.Failed);
            ResultBuilder.NewFeature("featureB").NewScenario(ExecutionStatus.Passed);
            ResultBuilder.NewFeature("featureC").NewScenario(ExecutionStatus.Bypassed);
            ResultBuilder.NewFeature("featureD").NewScenario(ExecutionStatus.Ignored);
            ResultBuilder.NewFeature("featureE").NewScenario(ExecutionStatus.Failed);
        }

        public async Task Given_a_html_report_is_created()
        {
            _features = ResultBuilder.Build();
            var htmlText = FormatResults(Features.ToArray());
            File.WriteAllText(HtmlFileName, htmlText);
        }

        public async Task When_a_html_report_is_opened()
        {
            _driver = await _driverHandle.ObtainAsync();
            Driver.Navigate().GoToUrl(HtmlFileName);
            Driver.EnsurePageIsLoaded();
            await StepExecution.Current.AttachFile(mgr => mgr.CreateFromData("message", "txt", Encoding.UTF8.GetBytes("some text")));
        }

        public async Task Then_all_features_should_be_VISIBLE([VisibleFormat] bool visible)
        {
            var actual = Driver.FindFeatures().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(Features.Count()));
        }

        public async Task Then_all_scenarios_should_be_VISIBLE([VisibleFormat] bool visible)
        {
            var actual = Driver.FindAllScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(Features.SelectMany(f => f.GetScenarios()).Count()));
        }

        public async Task Then_all_steps_should_be_VISIBLE([VisibleFormat] bool visible)
        {
            var actual = Driver.FindAllSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(Features.SelectMany(f => f.GetScenarios()).SelectMany(s => s.GetSteps()).Count()));
        }

        public async Task When_a_feature_collapse_button_is_clicked(int feature)
        {
            ClickLabeledButton(ToFeatureToggle(feature));
        }

        private void ClickLabeledButton(string buttonId)
        {
            FindLabeledButton(buttonId).Click();
            Driver.Synchronize();
        }

        private void ClickLabeledButtonSync(string buttonId)
        {
            FindLabeledButton(buttonId).ClickSync(Driver);
        }

        private IWebElement FindLabeledButton(string buttonId)
        {
            return Driver
                .FindElementsByTagName("label")
                .Single(l => l.GetAttribute("for") == buttonId);
        }

        private static string ToFeatureToggle(int feature)
        {
            return $"toggle{feature}";
        }

        public async Task Then_the_feature_scenarios_should_be_VISIBLE(int feature, [VisibleFormat] bool visible)
        {
            var actual = Driver.FindFeature(feature).FindScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(Features[feature - 1].GetScenarios().Count()));
        }

        public async Task When_a_feature_scenario_collapse_button_is_clicked(int feature, int scenario)
        {
            ClickLabeledButton(ToScenarioToggle(feature, scenario));
        }

        private static string ToScenarioToggle(int feature, int scenario)
        {
            return $"toggle{feature}_{scenario - 1}";
        }

        public async Task Then_the_feature_scenario_steps_should_be_VISIBLE(int feature, int scenario, [VisibleFormat] bool visible)
        {
            var actual = Driver.FindFeature(feature).FindScenario(scenario).FindSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(Features[feature - 1].GetScenarios().ElementAt(scenario - 1).GetSteps().Count()));
        }

        public async Task When_a_feature_filter_button_is_clicked()
        {
            ClickLabeledButtonSync("toggleFeatures");
        }

        public async Task When_a_scenario_filter_button_is_clicked()
        {
            ClickLabeledButtonSync("toggleScenarios");
        }

        public async Task Then_the_scenario_filter_button_should_be_SELECTED([SelectedFormat] bool selected)
        {
            Assert.That(Driver.FindElementById("toggleScenarios").Selected, Is.EqualTo(selected));
        }

        public async Task Then_the_feature_filter_button_should_be_SELECTED([SelectedFormat] bool selected)
        {
            Assert.That(Driver.FindElementById("toggleFeatures").Selected, Is.EqualTo(selected));
        }

        public async Task When_a_filter_status_button_is_clicked(ExecutionStatus status)
        {
            ClickLabeledButton($"show{status}");
        }

        public async Task Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus status, [SelectedFormat] bool selected)
        {
            Assert.That(Driver.FindElementById($"show{status}").Selected, Is.EqualTo(selected));
        }

        public async Task Then_all_scenarios_with_status_should_be_VISIBLE(ExecutionStatus status, [VisibleFormat] bool visible)
        {
            var elements = Driver.FindAllScenarios().Where(s => s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        public async Task Then_all_scenarios_with_status_other_than_STATUS_should_be_VISIBLE(ExecutionStatus status, [VisibleFormat] bool visible)
        {
            var elements = Driver.FindAllScenarios().Where(s => !s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        public async Task Then_all_features_having_all_scenarios_of_status_should_be_VISIBLE(ExecutionStatus status, [VisibleFormat] bool visible)
        {
            var expected = new[] { "feature", status.ToString().ToLower() };
            var elements = Driver.FindFeatures().Where(f => f.GetClassNames().SequenceEqual(expected)).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        public async Task When_a_category_filter_button_is_clicked(string category)
        {
            Driver.FindLabelByText(category)
                .ClickSync(Driver);
        }

        public async Task When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus status)
        {
            Driver.FindElementsByTagName("table").First(t => t.HasClassName("summary"))
                .FindElements(By.TagName("td"))
                .First(td => td.FindElements(By.TagName("span")).Any(span => span.HasClassName(status.ToString().ToLower() + "Alert")))
                .FindElements(By.TagName("a")).First()
                .Click();
        }

        public async Task Then_the_category_filter_button_should_be_SELECTED(string category, [SelectedFormat] bool selected)
        {
            var label = Driver.FindLabelByText(category);
            Assert.That(Driver.FindLabelTarget(label).Selected, Is.EqualTo(selected));
        }

        public async Task Then_the_feature_scenario_should_be_VISIBLE(int feature, int scenario, [VisibleFormat] bool visible)
        {
            Assert.That(Driver.FindFeature(feature).FindScenario(scenario).Displayed, Is.EqualTo(visible));
        }

        public async Task Then_the_feature_should_be_VISIBLE(int feature, [VisibleFormat] bool visible)
        {
            Assert.That(Driver.FindFeature(feature).Displayed, Is.EqualTo(visible));
        }

        public async Task Given_a_feature_result(string feature)
        {
            ResultBuilder.NewFeature(feature);
        }

        public async Task Given_the_feature_has_scenario_result_of_status_and_categories(string feature, ExecutionStatus status, params string[] categories)
        {
            ResultBuilder.ForFeature(feature).NewScenario(status).WithCategories(categories);
        }

        public async Task Given_the_feature_has_scenario_result_of_status(string feature, ExecutionStatus status)
        {
            ResultBuilder.ForFeature(feature).NewScenario(status);
        }

        public async Task Given_the_feature_has_scenario_result_of_status(string feature, string scenario, ExecutionStatus status)
        {
            ResultBuilder.ForFeature(feature).NewEmptyScenario(scenario, status);
        }

        public async Task Given_the_feature_scenario_has_step_result_with_status_and_tabular_parameter_and_content(string feature, string scenario, string step, ExecutionStatus status, string parameter, params (TableRowType type, string id, string name, string value)[] content)
        {
            var tabular = TestResults.CreateTabularParameterDetails(status == ExecutionStatus.Passed ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure)
                .WithKeyColumns("Id")
                .WithValueColumns("Name", "Value");

            foreach (var row in content)
            {
                tabular.AddRow(row.type,
                    ParameterVerificationStatus.Success,
                    TestResults.CreateValueResult(row.id),
                    TestResults.CreateValueResult(row.name, row.name, ParameterVerificationStatus.Success),
                    TestResults.CreateValueResult(row.value, row.value, ParameterVerificationStatus.Success));
            }

            ResultBuilder
                .ForFeature(feature)
                .ForScenario(scenario)
                .AddStep(step, status)
               .WithStepParameters(TestResults.CreateTestParameter(parameter, tabular));
        }

        public async Task Then_the_options_link_should_be_VISIBLE([VisibleFormat] bool visible)
        {
            Assert.That(Driver.FindElementById("optionsLink").Displayed, Is.EqualTo(visible));
        }

        public async Task When_the_options_link_is_clicked()
        {
            Driver
                .FindElementById("optionsLink")
                .Click();
        }

        public async Task Then_the_page_should_be_redirected_to_url_with_query_part()
        {
            Repeat.Until(
                () => Driver.Url.Contains("?"),
                () => $"Page was not redirected, actual value: {Driver.Url}");
            Driver.EnsurePageIsLoaded();
        }

        public async Task Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column([OrderFormat] bool ascending, FeatureSummaryColumn column)
        {
            var values = Driver
                .FindElementById("featuresSummary")
                .FindElements(By.TagName("tr"))
                .Skip(1)
                .Select(tr => tr.FindElements(By.TagName("td")).Where(td => td.Displayed).ElementAt((int)column - 1).Text)
                .ToArray();

            if (ascending)
                Assert.That(values, Is.EqualTo(values.OrderBy(v => v).ToArray()));
            else
                Assert.That(values, Is.EqualTo(values.OrderByDescending(v => v).ToArray()));
        }

        public async Task When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn column)
        {
            var webElement = Driver
                .FindElementById("featuresSummary")
                .FindElements(By.TagName("tr"))
                .First()
                .FindElements(By.TagName("th"))
                .Where(th => th.Displayed)
                .ElementAt((int)column - 1);

            Assert.That(column.ToString(), Does.Contain(webElement.Text));

            webElement.ClickSync(Driver);
        }

        public async Task Then_the_step_should_have_tabular_parameter_visible_with_rows(string step, string parameter, params (string id, string name, string value)[] rows)
        {
            var tableRows = Driver.FindAllSteps()
                .First(x => x.Text.Contains(step))
                .FindElement(By.ClassName("step-parameters"))
                .FindElements(By.ClassName("param")).First(e => e.FindElement(By.TagName("div")).Text.Equals($"{parameter}:"))
                .FindElement(By.TagName("table"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"));

            var actual = tableRows.Select(row => row.FindElements(By.TagName("td")).Skip(1).Select(x => x.Text).ToArray()).ToArray();

            Assert.That(actual, Is.EqualTo(rows.Select(r => new[] { r.id, r.name, r.value }).ToArray()));

            var screenShotPath = $"{Guid.NewGuid()}.PNG";
            Driver.GetScreenshot().SaveAsFile(screenShotPath, ScreenshotImageFormat.Png);
            await StepExecution.Current.AttachFile(mgr => mgr.CreateFromFile("screenshot", screenShotPath));
            await StepExecution.Current.AttachFile(mgr => mgr.CreateFromData("message", "txt", Encoding.UTF8.GetBytes("some text")));
        }

        private string FormatResults(params IFeatureResult[] results)
        {
            using (var memory = new MemoryStream())
            {
                new HtmlReportFormatter().Format(memory, results);
                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }
    }

    public enum FeatureSummaryColumn
    {
        Feature = 1,
        Scenarios,
        ScenariosPassed,
        ScenariosBypassed,
        ScenariosFailed,
        ScenariosIgnored,
        Steps,
        StepsPassed,
        StepsBypassed,
        StepsFailed,
        StepsIgnored,
        StepsNotRun,
        Duration,
        DurationAggregated,
        DurationAverage
    }
}