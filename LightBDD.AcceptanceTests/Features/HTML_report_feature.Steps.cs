using System;
using System.IO;
using System.Linq;
using System.Threading;
using LightBDD.AcceptanceTests.Helpers;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace LightBDD.AcceptanceTests.Features
{
    public partial class HTML_report_feature : FeatureFixture
    {
        private string _htmlFileName;
        private ChromeDriver _driver;
        private IFeatureResult[] _features;

        [TestFixtureSetUp]
        public void FeatureSetup()
        {
            _features = new[]
            {
                CreateFeature(ResultStatus.Passed, ResultStatus.Bypassed, ResultStatus.Ignored, ResultStatus.Failed,ResultStatus.NotRun),
                CreateFeature(ResultStatus.Passed),
                CreateFeature(ResultStatus.Bypassed), CreateFeature(ResultStatus.Ignored),
                CreateFeature(ResultStatus.Failed),
                CreateFeature(ResultStatus.NotRun)
            };

            var htmlText = new HtmlResultFormatter().Format(_features);
            _htmlFileName = Path.GetFullPath(Guid.NewGuid().ToString() + ".html");
            File.WriteAllText(_htmlFileName, htmlText);
            _driver = new ChromeDriver();
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

        private IFeatureResult CreateFeature(params ResultStatus[] scenarios)
        {
            return Mocks.CreateFeatureResult("feat", "descr", "label",
                scenarios.Select((s, i) => Mocks.CreateScenarioResult(
                    "scenario" + i.ToString(),
                    "label",
                    DateTimeOffset.Now,
                    TimeSpan.FromSeconds(2),
                    new[] { "categoryA", "categoryB" },
                    Mocks.CreateStepResult(1, "step1", ResultStatus.Passed, TimeSpan.FromSeconds(1)),
                    Mocks.CreateStepResult(2, "step2", s, TimeSpan.FromSeconds(1)))).ToArray());
        }

        private void an_opened_html_report()
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
            _driver.FindElementsByTagName("label").Single(l => l.GetAttribute("for") == buttonId).Click();
            Thread.Sleep(250);
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

        private void a_filter_button_is_clicked(ResultStatus filter)
        {
            ClickLabeledButton(string.Format("show{0}", filter));
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
    }
}