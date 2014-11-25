using LightBDD.Results;
using NUnit.Framework;

namespace LightBDD.AcceptanceTests.Features
{
    [FeatureDescription(
@"In order to analyze scenario test execution summary effectively
As a QA
I want to have HTML report")]
    [TestFixture]
    public partial class HTML_report_feature
    {
        [Test]
        public void Should_collapse_feature_details()
        {
            Runner.RunScenario(
                given => an_opened_html_report(),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_feature_collapse_button_is_clicked(1),
                then => the_feature_scenarios_are_VISIBLE(1, false),

                when => a_feature_collapse_button_is_clicked(1),
                then => the_feature_scenarios_are_VISIBLE(1, true));
        }

        [Test]
        public void Should_collapse_scenario_details()
        {
            Runner.RunScenario(
                given => an_opened_html_report(),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_feature_scenario_collapse_button_is_clicked(1, 1),
                then => the_feature_scenario_steps_are_VISIBLE(1, 1, false),

                when => a_feature_scenario_collapse_button_is_clicked(1, 1),
                then => the_feature_scenario_steps_are_VISIBLE(1, 1, true));

        }

        [Test]
        public void Should_collapse_all_features()
        {
            Runner.RunScenario(
                given => an_opened_html_report(),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_feature_filter_button_is_clicked(),
                and => all_features_are_VISIBLE(true),
                then => all_scenarios_are_VISIBLE(false),
                and => all_steps_are_VISIBLE(false),

                when => a_feature_filter_button_is_clicked(),
                then => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true));
        }

        [Test]
        public void Should_collapse_all_scenarios()
        {
            Runner.RunScenario(
                given => an_opened_html_report(),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_scenario_filter_button_is_clicked(),
               then => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(false),

                when => a_scenario_filter_button_is_clicked(),
                then => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true));
        }

        [Test]
        [TestCase(ResultStatus.Bypassed)]
        [TestCase(ResultStatus.Passed)]
        [TestCase(ResultStatus.Failed)]
        [TestCase(ResultStatus.Ignored)]
        public void Should_filter_by_status(ResultStatus status)
        {
            Runner.RunScenario(
                given => an_opened_html_report(),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_filter_button_is_clicked(status),
                then => all_scenarios_with_status_are_VISIBLE(status, false),
                and => all_scenarios_with_status_other_than_STATUS_are_VISIBLE(status, true),
                and => all_features_having_all_scenarios_of_status_are_VISIBLE(status, false),

                when => a_filter_button_is_clicked(status),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true)
                );
        }
    }
}