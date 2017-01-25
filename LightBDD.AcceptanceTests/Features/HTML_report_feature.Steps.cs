using System;
using System.IO;
using System.Linq;
using System.Text;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.AcceptanceTests.Helpers.Builders;
using LightBDD.Core.Results;
using LightBDD.Reporting.Formatters;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LightBDD.AcceptanceTests.Features
{
    public partial class HTML_report_feature : FeatureFixture
    {
        private string _htmlFileName;
        private ChromeDriver _driver;
        private IFeatureResult[] _features;
        private ResultBuilder _resultBuilder;

        [OneTimeSetUp]
        public void FeatureSetup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(250));
        }

        [SetUp]
        public void ScenarioSetUp()
        {
            _features = null;
            _htmlFileName = Path.GetFullPath(AppContext.BaseDirectory + "\\" + Guid.NewGuid() + ".html");
            _resultBuilder = new ResultBuilder();
        }

        [OneTimeTearDown]
        public void FeatureTearDown()
        {
            if (_driver != null)
            {
                _driver.Close();
                _driver.Dispose();
            }
        }

        private void a_various_features_with_scenarios_and_categories()
        {
            var feature = _resultBuilder.NewFeature("featureA");
            feature.NewScenario(ExecutionStatus.Passed).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Bypassed).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Ignored).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Failed).WithCategories("catA", "catB");
            _resultBuilder.NewFeature("featureB").NewScenario(ExecutionStatus.Passed);
            _resultBuilder.NewFeature("featureC").NewScenario(ExecutionStatus.Bypassed);
            _resultBuilder.NewFeature("featureD").NewScenario(ExecutionStatus.Ignored);
            _resultBuilder.NewFeature("featureE").NewScenario(ExecutionStatus.Failed);
        }

        private void a_various_features_with_scenarios_but_no_categories()
        {
            var feature = _resultBuilder.NewFeature("featureA");
            feature.NewScenario(ExecutionStatus.Passed);
            feature.NewScenario(ExecutionStatus.Bypassed);
            feature.NewScenario(ExecutionStatus.Ignored);
            feature.NewScenario(ExecutionStatus.Failed);
            _resultBuilder.NewFeature("featureB").NewScenario(ExecutionStatus.Passed);
            _resultBuilder.NewFeature("featureC").NewScenario(ExecutionStatus.Bypassed);
            _resultBuilder.NewFeature("featureD").NewScenario(ExecutionStatus.Ignored);
            _resultBuilder.NewFeature("featureE").NewScenario(ExecutionStatus.Failed);
        }

        private void a_html_report_is_created()
        {
            _features = _resultBuilder.Build();
            var htmlText = FormatResults(_features.ToArray());
            File.WriteAllText(_htmlFileName, htmlText);
        }

        private void a_html_report_is_opened()
        {
            _driver.Navigate().GoToUrl(_htmlFileName);
        }

        private void all_features_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = _driver.FindFeatures().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features.Count()));
        }

        private void all_scenarios_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = _driver.FindAllScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features.SelectMany(f => f.GetScenarios()).Count()));
        }

        private void all_steps_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = _driver.FindAllSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features.SelectMany(f => f.GetScenarios()).SelectMany(s => s.GetSteps()).Count()));
        }

        private void a_feature_collapse_button_is_clicked(int feature)
        {
            ClickLabeledButton(ToFeatureToggle(feature));
        }

        private void ClickLabeledButton(string buttonId)
        {
            _driver.FindElementsByTagName("label").Single(l => l.GetAttribute("for") == buttonId).Click();
        }

        private static string ToFeatureToggle(int feature)
        {
            return string.Format("toggle{0}", feature);
        }

        private void the_feature_scenarios_are_VISIBLE(int feature, [VisibleFormat]bool visible)
        {
            var actual = _driver.FindFeature(feature).FindScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features[feature - 1].GetScenarios().Count()));
        }

        private void a_feature_scenario_collapse_button_is_clicked(int feature, int scenario)
        {
            ClickLabeledButton(ToScenarioToggle(feature, scenario));
        }

        private static string ToScenarioToggle(int feature, int scenario)
        {
            return string.Format("toggle{0}_{1}", feature, scenario - 1);
        }

        private void the_feature_scenario_steps_are_VISIBLE(int feature, int scenario, [VisibleFormat]bool visible)
        {
            var actual = _driver.FindFeature(feature).FindScenario(scenario).FindSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features[feature - 1].GetScenarios().ElementAt(scenario - 1).GetSteps().Count()));
        }

        private void a_feature_filter_button_is_clicked()
        {
            ClickLabeledButton("toggleFeatures");
        }

        private void a_scenario_filter_button_is_clicked()
        {
            ClickLabeledButton("toggleScenarios");
        }

        private void the_scenario_filter_button_is_SELECTED([SelectedFormat]bool selected)
        {
            Assert.That(_driver.FindElementById("toggleScenarios").Selected, Is.EqualTo(selected));
        }

        private void the_feature_filter_button_is_SELECTED([SelectedFormat]bool selected)
        {
            Assert.That(_driver.FindElementById("toggleFeatures").Selected, Is.EqualTo(selected));
        }

        private void a_filter_status_button_is_clicked(ExecutionStatus status)
        {
            ClickLabeledButton(string.Format("show{0}", status));
        }

        private void the_filter_status_button_is_SELECTED(ExecutionStatus status, [SelectedFormat]bool selected)
        {
            Assert.That(_driver.FindElementById(string.Format("show{0}", status)).Selected, Is.EqualTo(selected));
        }

        private void all_scenarios_with_status_are_VISIBLE(ExecutionStatus status, [VisibleFormat]bool visible)
        {
            var elements = _driver.FindAllScenarios().Where(s => s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void all_scenarios_with_status_other_than_STATUS_are_VISIBLE(ExecutionStatus status, [VisibleFormat]bool visible)
        {
            var elements = _driver.FindAllScenarios().Where(s => !s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void all_features_having_all_scenarios_of_status_are_VISIBLE(ExecutionStatus status, [VisibleFormat]bool visible)
        {
            var expected = new[] { "feature", status.ToString().ToLower() };
            var elements = _driver.FindFeatures().Where(f => f.GetClassNames().SequenceEqual(expected)).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void a_category_filter_button_is_clicked(string category)
        {
            _driver.FindLabelByText(category).Click();
        }

        private void the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus status)
        {
            _driver.FindElementsByTagName("table").First(t => t.HasClassName("summary"))
                .FindElements(By.TagName("td")).First(td => td.FindElements(By.TagName("span")).Any(span => span.HasClassName(status.ToString().ToLower() + "Alert")))
                .FindElements(By.TagName("a")).First().Click();
        }

        private void the_category_filter_button_is_SELECTED(string category, [SelectedFormat]bool selected)
        {
            var label = _driver.FindLabelByText(category);
            Assert.That(_driver.FindLabelTarget(label).Selected, Is.EqualTo(selected));
        }

        private void the_feature_scenario_is_VISIBLE(int feature, int scenario, [VisibleFormat]bool visible)
        {
            Assert.That(_driver.FindFeature(feature).FindScenario(scenario).Displayed, Is.EqualTo(visible));
        }

        private void the_feature_is_VISIBLE(int feature, [VisibleFormat]bool visible)
        {
            Assert.That(_driver.FindFeature(feature).Displayed, Is.EqualTo(visible));
        }

        private void a_feature_result(string feature)
        {
            _resultBuilder.NewFeature(feature);
        }

        private void the_feature_has_scenario_result_of_status_and_categories(string feature, ExecutionStatus status, [ArrayFormat]params string[] categories)
        {
            _resultBuilder.ForFeature(feature).NewScenario(status).WithCategories(categories);
        }

        private void the_feature_has_scenario_result_of_status(string feature, ExecutionStatus status)
        {
            _resultBuilder.ForFeature(feature).NewScenario(status);
        }

        private void the_options_link_is_VISIBLE([VisibleFormat]bool visible)
        {
            Assert.That(_driver.FindElementById("optionsLink").Displayed, Is.EqualTo(visible));
        }

        private void the_options_link_is_clicked()
        {
            _driver.FindElementById("optionsLink").Click();
        }

        private void the_page_is_redirected_to_url_with_query_part()
        {
            Assert.That(_driver.Url, Does.Contain("?"));
        }

        private void the_Feature_Summary_table_is_sorted_ASCENDING_by_column([OrderFormat]bool ascending, FeatureSummaryColumn column)
        {
            var values = _driver
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

        private void the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn column)
        {
            var webElement = _driver
                .FindElementById("featuresSummary")
                .FindElements(By.TagName("tr"))
                .First()
                .FindElements(By.TagName("th"))
                .Where(th => th.Displayed)
                .ElementAt((int)column - 1);

            Assert.That(column.ToString(), Does.Contain(webElement.Text));

            webElement.Click();
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

    internal enum FeatureSummaryColumn
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