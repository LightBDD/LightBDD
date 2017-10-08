using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.AcceptanceTests.Helpers.Builders;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.NUnit3;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LightBDD.AcceptanceTests.Features
{
    public partial class HTML_report_feature : FeatureFixture
    {
        private class Context : IDisposable
        {
            public string HtmlFileName { get; }
            public ChromeDriver Driver { get; private set; }
            public IFeatureResult[] Features { get; set; }
            public ResultBuilder ResultBuilder { get; }

            public Context()
            {
                Driver = DriverPool.Acquire();
                HtmlFileName = Path.GetFullPath(BaseDirectory + "\\" + Guid.NewGuid() + ".html");
                ResultBuilder = new ResultBuilder();
            }

            public void Dispose()
            {
                if (Driver != null)
                    DriverPool.Release(Driver);
                Driver = null;
            }
        }

        private readonly AsyncLocal<Context> _context = new AsyncLocal<Context>();
        private Context ContextValue => _context.Value ?? throw new InvalidOperationException("No context");

        [SetUp]
        public void SetUp()
        {
            _context.Value = new Context();
        }

        [TearDown]
        public void TearDown()
        {
            ContextValue.Dispose();
        }

        private static string BaseDirectory
        {
            get
            {
#if NET451
                return AppDomain.CurrentDomain.BaseDirectory;
#else 
                return AppContext.BaseDirectory;
#endif
            }
        }

        private void a_various_features_with_scenarios_and_categories()
        {
            var feature = ContextValue.ResultBuilder.NewFeature("featureA");
            feature.NewScenario(ExecutionStatus.Passed).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Bypassed).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Ignored).WithCategories("catA", "catB");
            feature.NewScenario(ExecutionStatus.Failed).WithCategories("catA", "catB");
            ContextValue.ResultBuilder.NewFeature("featureB").NewScenario(ExecutionStatus.Passed);
            ContextValue.ResultBuilder.NewFeature("featureC").NewScenario(ExecutionStatus.Bypassed);
            ContextValue.ResultBuilder.NewFeature("featureD").NewScenario(ExecutionStatus.Ignored);
            ContextValue.ResultBuilder.NewFeature("featureE").NewScenario(ExecutionStatus.Failed);
        }

        private void a_various_features_with_scenarios_but_no_categories()
        {
            var feature = ContextValue.ResultBuilder.NewFeature("featureA");
            feature.NewScenario(ExecutionStatus.Passed);
            feature.NewScenario(ExecutionStatus.Bypassed);
            feature.NewScenario(ExecutionStatus.Ignored);
            feature.NewScenario(ExecutionStatus.Failed);
            ContextValue.ResultBuilder.NewFeature("featureB").NewScenario(ExecutionStatus.Passed);
            ContextValue.ResultBuilder.NewFeature("featureC").NewScenario(ExecutionStatus.Bypassed);
            ContextValue.ResultBuilder.NewFeature("featureD").NewScenario(ExecutionStatus.Ignored);
            ContextValue.ResultBuilder.NewFeature("featureE").NewScenario(ExecutionStatus.Failed);
        }

        private void a_html_report_is_created()
        {
            ContextValue.Features = ContextValue.ResultBuilder.Build();
            var htmlText = FormatResults(ContextValue.Features.ToArray());
            File.WriteAllText(ContextValue.HtmlFileName, htmlText);
        }

        private void a_html_report_is_opened()
        {
            ContextValue.Driver.Navigate().GoToUrl(ContextValue.HtmlFileName);
        }

        private void all_features_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = ContextValue.Driver.FindFeatures().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(ContextValue.Features.Count()));
        }

        private void all_scenarios_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = ContextValue.Driver.FindAllScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(ContextValue.Features.SelectMany(f => f.GetScenarios()).Count()));
        }

        private void all_steps_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = ContextValue.Driver.FindAllSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(ContextValue.Features.SelectMany(f => f.GetScenarios()).SelectMany(s => s.GetSteps()).Count()));
        }

        private void a_feature_collapse_button_is_clicked(int feature)
        {
            ClickLabeledButton(ToFeatureToggle(feature));
        }

        private void ClickLabeledButton(string buttonId)
        {
            ContextValue.Driver.FindElementsByTagName("label").Single(l => l.GetAttribute("for") == buttonId).Click();
        }

        private static string ToFeatureToggle(int feature)
        {
            return string.Format("toggle{0}", feature);
        }

        private void the_feature_scenarios_are_VISIBLE(int feature, [VisibleFormat]bool visible)
        {
            var actual = ContextValue.Driver.FindFeature(feature).FindScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(ContextValue.Features[feature - 1].GetScenarios().Count()));
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
            var actual = ContextValue.Driver.FindFeature(feature).FindScenario(scenario).FindSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(ContextValue.Features[feature - 1].GetScenarios().ElementAt(scenario - 1).GetSteps().Count()));
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
            Assert.That(ContextValue.Driver.FindElementById("toggleScenarios").Selected, Is.EqualTo(selected));
        }

        private void the_feature_filter_button_is_SELECTED([SelectedFormat]bool selected)
        {
            Assert.That(ContextValue.Driver.FindElementById("toggleFeatures").Selected, Is.EqualTo(selected));
        }

        private void a_filter_status_button_is_clicked(ExecutionStatus status)
        {
            ClickLabeledButton(string.Format("show{0}", status));
        }

        private void the_filter_status_button_is_SELECTED(ExecutionStatus status, [SelectedFormat]bool selected)
        {
            Assert.That(ContextValue.Driver.FindElementById(string.Format("show{0}", status)).Selected, Is.EqualTo(selected));
        }

        private void all_scenarios_with_status_are_VISIBLE(ExecutionStatus status, [VisibleFormat]bool visible)
        {
            var elements = ContextValue.Driver.FindAllScenarios().Where(s => s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void all_scenarios_with_status_other_than_STATUS_are_VISIBLE(ExecutionStatus status, [VisibleFormat]bool visible)
        {
            var elements = ContextValue.Driver.FindAllScenarios().Where(s => !s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void all_features_having_all_scenarios_of_status_are_VISIBLE(ExecutionStatus status, [VisibleFormat]bool visible)
        {
            var expected = new[] { "feature", status.ToString().ToLower() };
            var elements = ContextValue.Driver.FindFeatures().Where(f => f.GetClassNames().SequenceEqual(expected)).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void a_category_filter_button_is_clicked(string category)
        {
            ContextValue.Driver.FindLabelByText(category).Click();
        }

        private void the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus status)
        {
            ContextValue.Driver.FindElementsByTagName("table").First(t => t.HasClassName("summary"))
                .FindElements(By.TagName("td")).First(td => td.FindElements(By.TagName("span")).Any(span => span.HasClassName(status.ToString().ToLower() + "Alert")))
                .FindElements(By.TagName("a")).First().Click();
        }

        private void the_category_filter_button_is_SELECTED(string category, [SelectedFormat]bool selected)
        {
            var label = ContextValue.Driver.FindLabelByText(category);
            Assert.That(ContextValue.Driver.FindLabelTarget(label).Selected, Is.EqualTo(selected));
        }

        private void the_feature_scenario_is_VISIBLE(int feature, int scenario, [VisibleFormat]bool visible)
        {
            Assert.That(ContextValue.Driver.FindFeature(feature).FindScenario(scenario).Displayed, Is.EqualTo(visible));
        }

        private void the_feature_is_VISIBLE(int feature, [VisibleFormat]bool visible)
        {
            Assert.That(ContextValue.Driver.FindFeature(feature).Displayed, Is.EqualTo(visible));
        }

        private void a_feature_result(string feature)
        {
            ContextValue.ResultBuilder.NewFeature(feature);
        }

        private void the_feature_has_scenario_result_of_status_and_categories(string feature, ExecutionStatus status, params string[] categories)
        {
            ContextValue.ResultBuilder.ForFeature(feature).NewScenario(status).WithCategories(categories);
        }

        private void the_feature_has_scenario_result_of_status(string feature, ExecutionStatus status)
        {
            ContextValue.ResultBuilder.ForFeature(feature).NewScenario(status);
        }

        private void the_options_link_is_VISIBLE([VisibleFormat]bool visible)
        {
            Assert.That(ContextValue.Driver.FindElementById("optionsLink").Displayed, Is.EqualTo(visible));
        }

        private void the_options_link_is_clicked()
        {
            ContextValue.Driver.FindElementById("optionsLink").Click();
        }

        private void the_page_is_redirected_to_url_with_query_part()
        {
            Assert.That(ContextValue.Driver.Url, Does.Contain("?"));
        }

        private void the_Feature_Summary_table_is_sorted_ASCENDING_by_column([OrderFormat]bool ascending, FeatureSummaryColumn column)
        {
            var values = ContextValue.Driver
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
            var webElement = ContextValue.Driver
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