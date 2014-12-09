using System;
using System.IO;
using System.Linq;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.AcceptanceTests.Helpers.Builders;
using LightBDD.Results;
using LightBDD.Results.Formatters;
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

        [TestFixtureSetUp]
        public void FeatureSetup()
        {
            _driver = new ChromeDriver();
        }

        [SetUp]
        public void ScenarioSetUp()
        {
            _features = null;
            _htmlFileName = Path.GetFullPath(Guid.NewGuid() + ".html");
            _resultBuilder = new ResultBuilder();
        }

        [TestFixtureTearDown]
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
            feature.NewScenario(ResultStatus.Passed).WithCategories("catA", "catB");
            feature.NewScenario(ResultStatus.Bypassed).WithCategories("catA", "catB");
            feature.NewScenario(ResultStatus.Ignored).WithCategories("catA", "catB");
            feature.NewScenario(ResultStatus.Failed).WithCategories("catA", "catB");
            _resultBuilder.NewFeature("featureB").NewScenario(ResultStatus.Passed);
            _resultBuilder.NewFeature("featureC").NewScenario(ResultStatus.Bypassed);
            _resultBuilder.NewFeature("featureD").NewScenario(ResultStatus.Ignored);
            _resultBuilder.NewFeature("featureE").NewScenario(ResultStatus.Failed);
        }

        private void a_various_features_with_scenarios_but_no_categories()
        {
            var feature = _resultBuilder.NewFeature("featureA");
            feature.NewScenario(ResultStatus.Passed);
            feature.NewScenario(ResultStatus.Bypassed);
            feature.NewScenario(ResultStatus.Ignored);
            feature.NewScenario(ResultStatus.Failed);
            _resultBuilder.NewFeature("featureB").NewScenario(ResultStatus.Passed);
            _resultBuilder.NewFeature("featureC").NewScenario(ResultStatus.Bypassed);
            _resultBuilder.NewFeature("featureD").NewScenario(ResultStatus.Ignored);
            _resultBuilder.NewFeature("featureE").NewScenario(ResultStatus.Failed);
        }

        private void a_html_report_is_created()
        {
            _features = _resultBuilder.Build();
            var htmlText = new HtmlResultFormatter().Format(_features.ToArray());
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
            Assert.That(actual, Is.EqualTo(_features.SelectMany(f => f.Scenarios).Count()));
        }

        private void all_steps_are_VISIBLE([VisibleFormat]bool visible)
        {
            var actual = _driver.FindAllSteps().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features.SelectMany(f => f.Scenarios).SelectMany(s => s.Steps).Count()));
        }

        private void a_feature_collapse_button_is_clicked(int feature)
        {
            ClickLabeledButton(ToFeatureToggle(feature));
        }

        private void ClickLabeledButton(string buttonId)
        {
            _driver.FindElementsByTagName("label").Single(l => l.GetAttribute("for") == buttonId).ClickWithWait();
        }

        private static string ToFeatureToggle(int feature)
        {
            return string.Format("toggle{0}", feature);
        }

        private void the_feature_scenarios_are_VISIBLE(int feature, [VisibleFormat]bool visible)
        {
            var actual = _driver.FindFeature(feature).FindScenarios().Count(e => e.Displayed == visible);
            Assert.That(actual, Is.EqualTo(_features[feature - 1].Scenarios.Count()));
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
            Assert.That(actual, Is.EqualTo(_features[feature - 1].Scenarios.ElementAt(scenario - 1).Steps.Count()));
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

        private void a_filter_status_button_is_clicked(ResultStatus status)
        {
            ClickLabeledButton(string.Format("show{0}", status));
        }

        private void the_filter_status_button_is_SELECTED(ResultStatus status, [SelectedFormat]bool selected)
        {
            Assert.That(_driver.FindElementById(string.Format("show{0}", status)).Selected, Is.EqualTo(selected));
        }

        private void all_scenarios_with_status_are_VISIBLE(ResultStatus status, [VisibleFormat]bool visible)
        {
            var elements = _driver.FindAllScenarios().Where(s => s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void all_scenarios_with_status_other_than_STATUS_are_VISIBLE(ResultStatus status, [VisibleFormat]bool visible)
        {
            var elements = _driver.FindAllScenarios().Where(s => !s.HasClassName(status.ToString().ToLower())).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void all_features_having_all_scenarios_of_status_are_VISIBLE(ResultStatus status, [VisibleFormat]bool visible)
        {
            var expected = new[] { "feature", status.ToString().ToLower() };
            var elements = _driver.FindFeatures().Where(f => f.GetClassNames().SequenceEqual(expected)).ToArray();
            Assert.That(elements, Is.Not.Empty);
            Assert.That(elements.All(s => s.Displayed == visible));
        }

        private void a_category_filter_button_is_clicked(string category)
        {
            _driver.FindLabelByText(category).ClickWithWait();
        }

        private void the_link_to_details_of_STATUS_scenarios_is_clicked(ResultStatus status)
        {
            _driver.FindElementsByTagName("table").First(t => t.HasClassName("summary"))
                .FindElements(By.TagName("td")).First(td => td.FindElements(By.TagName("span")).Any(span => span.HasClassName(status.ToString().ToLower() + "Alert")))
                .FindElements(By.TagName("a")).First().ClickWithWait();
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

        private void the_feature_has_scenario_result_of_status_and_categories(string feature, ResultStatus status, [ArrayFormat]params string[] categories)
        {
            _resultBuilder.ForFeature(feature).NewScenario(status).WithCategories(categories);
        }

        private void the_feature_has_scenario_result_of_status(string feature, ResultStatus status)
        {
            _resultBuilder.ForFeature(feature).NewScenario(status);
        }

        private void the_options_link_is_VISIBLE([VisibleFormat]bool visible)
        {
            Assert.That(_driver.FindElementById("optionsLink").Displayed, Is.EqualTo(visible));
        }

        private void the_options_link_is_clicked()
        {
            _driver.FindElementById("optionsLink").ClickWithWait();
        }

        private void the_page_is_redirected_to_url_with_query_part()
        {
            Assert.That(_driver.Url, Is.StringContaining("?"));
        }

        private void the_Feature_Summary_table_is_sorted_ASCENDING_by_column([OrderFormat]bool ascending, int column)
        {
            var values = _driver
                .FindElementById("featuresSummary")
                .FindElements(By.TagName("tr"))
                .Skip(1)
                .Select(tr => tr.FindElements(By.TagName("td")).Where(td => td.Displayed).ElementAt(column - 1).Text)
                .ToArray();

            if (ascending)
                Assert.That(values, Is.EqualTo(values.OrderBy(v => v).ToArray()));
            else
                Assert.That(values, Is.EqualTo(values.OrderByDescending(v => v).ToArray()));
        }

        private void the_Feature_Summary_table_column_is_clicked(int column)
        {
            _driver
                .FindElementById("featuresSummary")
                .FindElements(By.TagName("tr"))
                .First()
                .FindElements(By.TagName("th"))
                .Where(th => th.Displayed)
                .ElementAt(column - 1)
                .ClickWithWait();
        }
    }
}