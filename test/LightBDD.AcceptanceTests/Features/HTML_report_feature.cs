using System;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.NUnit3;
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
        [Scenario]
        public void Should_collapse_feature_details()
        {
            Runner.RunScenario(
                _ => Given_a_various_features_with_scenarios_and_categories(),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_feature_collapse_button_is_clicked(1),
                _ => Then_the_feature_scenarios_should_be_VISIBLE(1, false),

                _ => When_a_feature_collapse_button_is_clicked(1),
                _ => Then_the_feature_scenarios_should_be_VISIBLE(1, true));
        }

        [Scenario]
        public void Should_collapse_scenario_details()
        {
            Runner.RunScenario(
                _ => Given_a_various_features_with_scenarios_and_categories(),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_feature_scenario_collapse_button_is_clicked(1, 1),
                _ => Then_the_feature_scenario_steps_should_be_VISIBLE(1, 1, false),

                _ => When_a_feature_scenario_collapse_button_is_clicked(1, 1),
                _ => Then_the_feature_scenario_steps_should_be_VISIBLE(1, 1, true));

        }

        [Scenario]
        public void Should_collapse_all_features()
        {
            Runner.RunScenario(
                _ => Given_a_various_features_with_scenarios_and_categories(),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_feature_filter_button_is_clicked(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(false),
                _ => Then_all_steps_should_be_VISIBLE(false),

                _ => When_a_feature_filter_button_is_clicked(),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true));
        }

        [Scenario]
        public void Should_collapse_all_scenarios()
        {
            Runner.RunScenario(
                _ => Given_a_various_features_with_scenarios_and_categories(),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_scenario_filter_button_is_clicked(),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(false),

                _ => When_a_scenario_filter_button_is_clicked(),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true));
        }

        [Scenario]
        [TestCase(ExecutionStatus.Bypassed)]
        [TestCase(ExecutionStatus.Passed)]
        [TestCase(ExecutionStatus.Failed)]
        [TestCase(ExecutionStatus.Ignored)]
        public void Should_filter_by_status(ExecutionStatus status)
        {
            Runner.RunScenario(
                _ => Given_a_various_features_with_scenarios_and_categories(),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_filter_status_button_is_clicked(status),
                _ => Then_all_scenarios_with_status_should_be_VISIBLE(status, false),
                _ => Then_all_scenarios_with_status_other_than_STATUS_should_be_VISIBLE(status, true),
                _ => Then_all_features_having_all_scenarios_of_status_should_be_VISIBLE(status, false),

                _ => When_a_filter_status_button_is_clicked(status),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true)
            );
        }

        [Scenario]
        [TestCase(ExecutionStatus.Bypassed)]
        [TestCase(ExecutionStatus.Passed)]
        [TestCase(ExecutionStatus.Failed)]
        [TestCase(ExecutionStatus.Ignored)]
        public void Should_filter_by_status_when_there_is_no_categories_filter_bar(ExecutionStatus status)
        {
            Runner.RunScenario(
                _ => Given_a_various_features_with_scenarios_but_no_categories(),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_filter_status_button_is_clicked(status),
                _ => Then_all_scenarios_with_status_should_be_VISIBLE(status, false),
                _ => Then_all_scenarios_with_status_other_than_STATUS_should_be_VISIBLE(status, true),
                _ => Then_all_features_having_all_scenarios_of_status_should_be_VISIBLE(status, false),

                _ => When_a_filter_status_button_is_clicked(status),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true)
            );
        }

        [Scenario]
        public void Should_filter_by_category()
        {
            Runner.RunScenario(
                _ => Given_a_feature_result("featureA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA", "catB"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                _ => Given_a_feature_result("featureB"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Passed, "catA"),
                _ => Given_a_feature_result("featureC"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                _ => Given_a_feature_result("featureD"),
                _ => Given_the_feature_has_scenario_result_of_status("featureD", ExecutionStatus.Passed),
                _ => Given_a_feature_result("featureE"),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_category_filter_button_is_clicked("catA"),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),

                _ => Then_the_feature_scenario_should_be_VISIBLE(2, 1, true),
                _ => Then_the_feature_should_be_VISIBLE(3, false),
                _ => Then_the_feature_should_be_VISIBLE(4, false),
                _ => Then_the_feature_should_be_VISIBLE(5, false),

                _ => When_a_category_filter_button_is_clicked("catB"),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, true),

                _ => Then_the_feature_should_be_VISIBLE(2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(3, 1, true),
                _ => Then_the_feature_should_be_VISIBLE(4, false),
                _ => Then_the_feature_should_be_VISIBLE(5, false),

                _ => When_a_category_filter_button_is_clicked("-without category-"),
                _ => Then_the_feature_should_be_VISIBLE(1, false),
                _ => Then_the_feature_should_be_VISIBLE(2, false),
                _ => Then_the_feature_should_be_VISIBLE(3, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(4, 1, true),
                _ => Then_the_feature_should_be_VISIBLE(5, false),

                _ => When_a_category_filter_button_is_clicked("-all-"),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true)
            );
        }

        [Scenario]
        public void Should_filter_by_category_and_status()
        {
            Runner.RunScenario(
                _ => Given_a_feature_result("featureA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Failed, "catA", "catB"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                _ => Given_a_feature_result("featureB"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Ignored, "catA"),
                _ => Given_a_feature_result("featureC"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_a_category_filter_button_is_clicked("catA"),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(2, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(3, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(3, 2, false),

                _ => When_a_filter_status_button_is_clicked(ExecutionStatus.Passed),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(2, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(3, 1, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(3, 2, false),
                _ => Then_the_feature_should_be_VISIBLE(3, false),

                _ => When_a_filter_status_button_is_clicked(ExecutionStatus.Ignored),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_the_feature_should_be_VISIBLE(2, false),
                _ => Then_the_feature_should_be_VISIBLE(3, false)
            );
        }

        [Scenario]
        public void Should_follow_shareable_link_with_preserving_selected_options()
        {
            Runner.RunScenario(
                _ => Given_a_feature_result("featureA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Failed, "catA", "catB"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                _ => Given_a_feature_result("featureB"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Ignored, "catA"),
                _ => Given_a_feature_result("featureC"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catA"),
                _ => Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),
                _ => Then_the_options_link_should_be_VISIBLE(false),

                _ => When_a_category_filter_button_is_clicked("catA"),
                _ => When_a_filter_status_button_is_clicked(ExecutionStatus.Passed),
                _ => When_a_filter_status_button_is_clicked(ExecutionStatus.Ignored),
                _ => When_a_scenario_filter_button_is_clicked(),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_the_feature_should_be_VISIBLE(2, false),
                _ => Then_the_feature_should_be_VISIBLE(3, false),
                _ => Then_all_steps_should_be_VISIBLE(false),
                _ => Then_the_options_link_should_be_VISIBLE(true),

                _ => When_the_options_link_is_clicked(),
                _ => Then_the_page_should_be_redirected_to_url_with_query_part(),
                _ => Then_the_category_filter_button_should_be_SELECTED("catA", true),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, false),
                _ => Then_the_scenario_filter_button_should_be_SELECTED(false),

                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_the_feature_should_be_VISIBLE(2, false),
                _ => Then_the_feature_should_be_VISIBLE(3, false)
            );
        }

        [Scenario]
        public void Should_sort_feature_summary_rows()
        {
            Runner.RunScenario(
                _ => Given_a_feature_result("a"),
                _ => Given_the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                _ => Given_the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                _ => Given_the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                _ => Given_a_feature_result("b"),
                _ => Given_the_feature_has_scenario_result_of_status("b", ExecutionStatus.Passed),
                _ => Given_the_feature_has_scenario_result_of_status("b", ExecutionStatus.Passed),
                _ => Given_a_feature_result("c"),
                _ => Given_the_feature_has_scenario_result_of_status("c", ExecutionStatus.Passed),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Feature),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Feature),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Feature),

                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Scenarios),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Scenarios),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Scenarios),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Scenarios),

                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.ScenariosPassed),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.ScenariosPassed),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.ScenariosPassed),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.ScenariosPassed),

                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Steps),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Steps),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Steps),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Steps),

                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Duration),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Duration),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Duration),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Duration),

                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAggregated),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.DurationAggregated),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAggregated),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.DurationAggregated),

                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAverage),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.DurationAverage),
                _ => When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAverage),
                _ => Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.DurationAverage)
            );
        }

        [Scenario]
        public void Should_show_details_of_non_passed_scenarios()
        {
            Runner.RunScenario(
                _ => Given_a_feature_result("featureA"),
                _ => Given_the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Failed),
                _ => Given_the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Ignored),
                _ => Given_the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Bypassed),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_all_features_should_be_VISIBLE(true),
                _ => Then_all_scenarios_should_be_VISIBLE(true),
                _ => Then_all_steps_should_be_VISIBLE(true),

                _ => When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Failed),
                _ => Then_the_page_should_be_redirected_to_url_with_query_part(),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Failed, true),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Bypassed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.NotRun, false),
                _ => Then_the_feature_filter_button_should_be_SELECTED(true),
                _ => Then_the_scenario_filter_button_should_be_SELECTED(false),

                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_all_steps_should_be_VISIBLE(false),

                _ => When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Ignored),
                _ => Then_the_page_should_be_redirected_to_url_with_query_part(),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Failed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, true),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Bypassed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.NotRun, false),
                _ => Then_the_feature_filter_button_should_be_SELECTED(true),
                _ => Then_the_scenario_filter_button_should_be_SELECTED(false),

                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, true),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                _ => Then_all_steps_should_be_VISIBLE(false),

                _ => When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Bypassed),
                _ => Then_the_page_should_be_redirected_to_url_with_query_part(),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Failed, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, false),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Bypassed, true),
                _ => Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.NotRun, false),
                _ => Then_the_feature_filter_button_should_be_SELECTED(true),
                _ => Then_the_scenario_filter_button_should_be_SELECTED(false),

                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 1, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                _ => Then_the_feature_scenario_should_be_VISIBLE(1, 3, true),
                _ => Then_all_steps_should_be_VISIBLE(false));
        }

        [Scenario]
        public void Should_show_tabular_parameters()
        {
            Runner.RunScenario(
                _ => Given_a_feature_result("feat1"),
                _ => Given_the_feature_has_scenario_result_of_status("feat1", "scenario1", ExecutionStatus.Failed),
                _ => Given_the_feature_scenario_has_step_result_with_status_and_tabular_parameter_and_content("feat1", "scenario1", "step1", ExecutionStatus.Passed, "param1",
                    ValueTuple.Create(TableRowType.Matching, "id1", "name1", "value1"),
                    ValueTuple.Create(TableRowType.Matching, "id2", "name2", "value2")),
                _ => Given_a_html_report_is_created(),

                _ => When_a_html_report_is_opened(),
                _ => Then_the_step_should_have_tabular_parameter_visible_with_rows("step1", "param1",
                    ValueTuple.Create("id1", "name1", "value1"),
                    ValueTuple.Create("id2", "name2", "value2"))
                );
        }
    }
}