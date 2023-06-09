using System;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.NUnit3;
using NUnit.Framework;

namespace LightBDD.AcceptanceTests.Features
{
    [FeatureDescription(
@"In order to analyze scenario test execution summary effectively
As a QA
I want to have HTML report")]
    [TestFixture]
    public class HTML_report_feature : FeatureFixture
    {
        [Scenario]
        public async Task Should_collapse_feature_details()
        {
            await Runner
                 .WithContext<HtmlReportContext>()
                 .RunScenarioAsync(
                 x => x.Given_a_various_features_with_scenarios_and_categories(),
                 x => x.Given_a_html_report_is_created(),

                 x => x.When_a_html_report_is_opened(),
                 x => x.Then_all_features_should_be_VISIBLE(true),
                 x => x.Then_all_scenarios_should_be_VISIBLE(true),
                 x => x.Then_all_steps_should_be_VISIBLE(true),

                 x => x.When_a_feature_collapse_button_is_clicked(1),
                 x => x.Then_the_feature_scenarios_should_be_VISIBLE(1, false),

                 x => x.When_a_feature_collapse_button_is_clicked(1),
                 x => x.Then_the_feature_scenarios_should_be_VISIBLE(1, true));
        }

        [Scenario]
        public async Task Should_collapse_scenario_details()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_various_features_with_scenarios_and_categories(),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),

                x => x.When_a_feature_scenario_collapse_button_is_clicked(1, 1),
                x => x.Then_the_feature_scenario_steps_should_be_VISIBLE(1, 1, false),

                x => x.When_a_feature_scenario_collapse_button_is_clicked(1, 1),
                x => x.Then_the_feature_scenario_steps_should_be_VISIBLE(1, 1, true));

        }

        [Scenario]
        public async Task Should_expand_scenario_step_sub_steps()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                    x => x.Given_a_various_features_with_scenarios_and_categories(),
                    x => x.Given_a_html_report_is_created(),

                    x => x.When_a_html_report_is_opened(),
                    x => x.Then_all_features_should_be_VISIBLE(true),
                    x => x.Then_all_scenarios_should_be_VISIBLE(true),
                    x => x.Then_all_steps_should_be_VISIBLE(true),
                    x => x.Then_all_sub_steps_should_be_VISIBLE(false),

                    x => x.When_a_feature_scenario_step_collapse_button_is_clicked(1, 1, 1),
                    x => x.Then_the_feature_scenario_step_sub_steps_should_be_VISIBLE(1, 1, 1, true),

                    x => x.When_a_feature_scenario_step_collapse_button_is_clicked(1, 1, 1),
                    x => x.Then_the_feature_scenario_step_sub_steps_should_be_VISIBLE(1, 1, 1, false));

        }

        [Scenario]
        public async Task Should_collapse_all_features()
        {
            await Runner
                 .WithContext<HtmlReportContext>()
                 .RunScenarioAsync(
                 x => x.Given_a_various_features_with_scenarios_and_categories(),
                 x => x.Given_a_html_report_is_created(),

                 x => x.When_a_html_report_is_opened(),
                 x => x.Then_all_features_should_be_VISIBLE(true),
                 x => x.Then_all_scenarios_should_be_VISIBLE(true),
                 x => x.Then_all_steps_should_be_VISIBLE(true),

                 x => x.When_a_feature_filter_button_is_clicked(),
                 x => x.Then_all_features_should_be_VISIBLE(true),
                 x => x.Then_all_scenarios_should_be_VISIBLE(false),
                 x => x.Then_all_steps_should_be_VISIBLE(false),

                 x => x.When_a_feature_filter_button_is_clicked(),
                 x => x.Then_all_scenarios_should_be_VISIBLE(true),
                 x => x.Then_all_steps_should_be_VISIBLE(true));
        }

        [Scenario]
        public async Task Should_collapse_all_scenarios()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_various_features_with_scenarios_and_categories(),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),

                x => x.When_a_scenario_filter_button_is_clicked(),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(false),

                x => x.When_a_scenario_filter_button_is_clicked(),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true));
        }

        [Scenario]
        public async Task Should_expand_all_sub_steps()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                    x => x.Given_a_various_features_with_scenarios_and_categories(),
                    x => x.Given_a_html_report_is_created(),

                    x => x.When_a_html_report_is_opened(),
                    x => x.Then_all_features_should_be_VISIBLE(true),
                    x => x.Then_all_scenarios_should_be_VISIBLE(true),
                    x => x.Then_all_steps_should_be_VISIBLE(true),
                    x => x.Then_all_sub_steps_should_be_VISIBLE(false),

                    x => x.When_a_sub_step_filter_button_is_clicked(),
                    x => x.Then_all_scenarios_should_be_VISIBLE(true),
                    x => x.Then_all_steps_should_be_VISIBLE(true),
                    x => x.Then_all_sub_steps_should_be_VISIBLE(true),

                    x => x.When_a_sub_step_filter_button_is_clicked(),
                    x => x.Then_all_scenarios_should_be_VISIBLE(true),
                    x => x.Then_all_steps_should_be_VISIBLE(true),
                    x => x.Then_all_sub_steps_should_be_VISIBLE(false));
        }

        [Scenario]
        [TestCase(ExecutionStatus.Bypassed)]
        [TestCase(ExecutionStatus.Passed)]
        [TestCase(ExecutionStatus.Failed)]
        [TestCase(ExecutionStatus.Ignored)]
        public async Task Should_filter_by_status(ExecutionStatus status)
        {
            await Runner
                 .WithContext<HtmlReportContext>()
                 .RunScenarioAsync(
                 x => x.Given_a_various_features_with_scenarios_and_categories(),
                 x => x.Given_a_html_report_is_created(),

                 x => x.When_a_html_report_is_opened(),
                 x => x.Then_all_features_should_be_VISIBLE(true),
                 x => x.Then_all_scenarios_should_be_VISIBLE(true),
                 x => x.Then_all_steps_should_be_VISIBLE(true),

                 x => x.When_a_filter_status_button_is_clicked(status),
                 x => x.Then_all_scenarios_with_status_should_be_VISIBLE(status, false),
                 x => x.Then_all_scenarios_with_status_other_than_STATUS_should_be_VISIBLE(status, true),
                 x => x.Then_all_features_having_all_scenarios_of_status_should_be_VISIBLE(status, false),

                 x => x.When_a_filter_status_button_is_clicked(status),
                 x => x.Then_all_features_should_be_VISIBLE(true),
                 x => x.Then_all_scenarios_should_be_VISIBLE(true),
                 x => x.Then_all_steps_should_be_VISIBLE(true)
             );
        }

        [Scenario]
        [TestCase(ExecutionStatus.Bypassed)]
        [TestCase(ExecutionStatus.Passed)]
        [TestCase(ExecutionStatus.Failed)]
        [TestCase(ExecutionStatus.Ignored)]
        public async Task Should_filter_by_status_when_there_is_no_categories_filter_bar(ExecutionStatus status)
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_various_features_with_scenarios_but_no_categories(),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),

                x => x.When_a_filter_status_button_is_clicked(status),
                x => x.Then_all_scenarios_with_status_should_be_VISIBLE(status, false),
                x => x.Then_all_scenarios_with_status_other_than_STATUS_should_be_VISIBLE(status, true),
                x => x.Then_all_features_having_all_scenarios_of_status_should_be_VISIBLE(status, false),

                x => x.When_a_filter_status_button_is_clicked(status),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true)
            );
        }

        [Scenario]
        public async Task Should_filter_by_category()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_feature_result("featureA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA", "catB"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                x => x.Given_a_feature_result("featureB"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Passed, "catA"),
                x => x.Given_a_feature_result("featureC"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                x => x.Given_a_feature_result("featureD"),
                x => x.Given_the_feature_has_scenario_result_of_status("featureD", ExecutionStatus.Passed),
                x => x.Given_a_feature_result("featureE"),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),

                x => x.When_a_category_filter_button_is_clicked("catA"),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),

                x => x.Then_the_feature_scenario_should_be_VISIBLE(2, 1, true),
                x => x.Then_the_feature_should_be_VISIBLE(3, false),
                x => x.Then_the_feature_should_be_VISIBLE(4, false),
                x => x.Then_the_feature_should_be_VISIBLE(5, false),

                x => x.When_a_category_filter_button_is_clicked("catB"),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, true),

                x => x.Then_the_feature_should_be_VISIBLE(2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(3, 1, true),
                x => x.Then_the_feature_should_be_VISIBLE(4, false),
                x => x.Then_the_feature_should_be_VISIBLE(5, false),

                x => x.When_a_category_filter_button_is_clicked("-without category-"),
                x => x.Then_the_feature_should_be_VISIBLE(1, false),
                x => x.Then_the_feature_should_be_VISIBLE(2, false),
                x => x.Then_the_feature_should_be_VISIBLE(3, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(4, 1, true),
                x => x.Then_the_feature_should_be_VISIBLE(5, false),

                x => x.When_a_category_filter_button_is_clicked("-all-"),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true)
            );
        }

        [Scenario]
        public async Task Should_filter_by_category_and_status()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_feature_result("featureA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Failed, "catA", "catB"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                x => x.Given_a_feature_result("featureB"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Ignored, "catA"),
                x => x.Given_a_feature_result("featureC"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),

                x => x.When_a_category_filter_button_is_clicked("catA"),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(2, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(3, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(3, 2, false),

                x => x.When_a_filter_status_button_is_clicked(ExecutionStatus.Passed),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(2, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(3, 1, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(3, 2, false),
                x => x.Then_the_feature_should_be_VISIBLE(3, false),

                x => x.When_a_filter_status_button_is_clicked(ExecutionStatus.Ignored),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_the_feature_should_be_VISIBLE(2, false),
                x => x.Then_the_feature_should_be_VISIBLE(3, false)
            );
        }

        [Scenario]
        public async Task Should_follow_shareable_link_with_preserving_selected_options()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_feature_result("featureA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Failed, "catA", "catB"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                x => x.Given_a_feature_result("featureB"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Ignored, "catA"),
                x => x.Given_a_feature_result("featureC"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catA"),
                x => x.Given_the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),
                x => x.Then_the_options_link_should_be_VISIBLE(false),

                x => x.When_a_category_filter_button_is_clicked("catA"),
                x => x.When_a_filter_status_button_is_clicked(ExecutionStatus.Passed),
                x => x.When_a_filter_status_button_is_clicked(ExecutionStatus.Ignored),
                x => x.When_a_scenario_filter_button_is_clicked(),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_the_feature_should_be_VISIBLE(2, false),
                x => x.Then_the_feature_should_be_VISIBLE(3, false),
                x => x.Then_all_steps_should_be_VISIBLE(false),
                x => x.Then_the_options_link_should_be_VISIBLE(true),

                x => x.When_the_options_link_is_clicked(),
                x => x.Then_the_page_should_be_redirected_to_url_with_query_part(),
                x => x.Then_the_category_filter_button_should_be_SELECTED("catA", true),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, false),
                x => x.Then_the_scenario_filter_button_should_be_SELECTED(false),

                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_the_feature_should_be_VISIBLE(2, false),
                x => x.Then_the_feature_should_be_VISIBLE(3, false)
            );
        }

        [Scenario]
        public async Task Should_sort_feature_summary_rows()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_feature_result("a"),
                x => x.Given_the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                x => x.Given_the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                x => x.Given_the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                x => x.Given_a_feature_result("b"),
                x => x.Given_the_feature_has_scenario_result_of_status("b", ExecutionStatus.Passed),
                x => x.Given_the_feature_has_scenario_result_of_status("b", ExecutionStatus.Passed),
                x => x.Given_a_feature_result("c"),
                x => x.Given_the_feature_has_scenario_result_of_status("c", ExecutionStatus.Passed),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Feature),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Feature),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Feature),

                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Scenarios),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Scenarios),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Scenarios),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Scenarios),

                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.ScenariosPassed),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.ScenariosPassed),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.ScenariosPassed),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.ScenariosPassed),

                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Steps),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Steps),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Steps),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Steps),

                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Duration),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.Duration),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.Duration),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.Duration),

                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAggregated),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.DurationAggregated),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAggregated),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.DurationAggregated),

                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAverage),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(false, FeatureSummaryColumn.DurationAverage),
                x => x.When_the_Feature_Summary_table_column_is_clicked(FeatureSummaryColumn.DurationAverage),
                x => x.Then_the_Feature_Summary_table_should_be_sorted_ASCENDING_by_column(true, FeatureSummaryColumn.DurationAverage)
            );
        }

        [Scenario]
        public async Task Should_show_details_of_non_passed_scenarios()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_feature_result("featureA"),
                x => x.Given_the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Failed),
                x => x.Given_the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Ignored),
                x => x.Given_the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Bypassed),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_all_features_should_be_VISIBLE(true),
                x => x.Then_all_scenarios_should_be_VISIBLE(true),
                x => x.Then_all_steps_should_be_VISIBLE(true),

                x => x.When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Failed),
                x => x.Then_the_page_should_be_redirected_to_url_with_query_part(),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Failed, true),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Bypassed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.NotRun, false),
                x => x.Then_the_feature_filter_button_should_be_SELECTED(true),
                x => x.Then_the_scenario_filter_button_should_be_SELECTED(false),

                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_all_steps_should_be_VISIBLE(false),

                x => x.When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Ignored),
                x => x.Then_the_page_should_be_redirected_to_url_with_query_part(),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Failed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, true),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Bypassed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.NotRun, false),
                x => x.Then_the_feature_filter_button_should_be_SELECTED(true),
                x => x.Then_the_scenario_filter_button_should_be_SELECTED(false),

                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, true),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, false),
                x => x.Then_all_steps_should_be_VISIBLE(false),

                x => x.When_the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Bypassed),
                x => x.Then_the_page_should_be_redirected_to_url_with_query_part(),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Passed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Failed, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Ignored, false),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.Bypassed, true),
                x => x.Then_the_filter_status_button_should_be_SELECTED(ExecutionStatus.NotRun, false),
                x => x.Then_the_feature_filter_button_should_be_SELECTED(true),
                x => x.Then_the_scenario_filter_button_should_be_SELECTED(false),

                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 1, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 2, false),
                x => x.Then_the_feature_scenario_should_be_VISIBLE(1, 3, true),
                x => x.Then_all_steps_should_be_VISIBLE(false));
        }

        [Scenario]
        public async Task Should_show_tabular_parameters()
        {
            await Runner
                .WithContext<HtmlReportContext>()
                .RunScenarioAsync(
                x => x.Given_a_feature_result("feat1"),
                x => x.Given_the_feature_has_scenario_result_of_status("feat1", "scenario1", ExecutionStatus.Failed),
                x => x.Given_the_feature_scenario_has_step_result_with_status_and_tabular_parameter_and_content("feat1", "scenario1", "step1", ExecutionStatus.Passed, "param1",
                    ValueTuple.Create(TableRowType.Matching, "id1", "name1", "value1"),
                    ValueTuple.Create(TableRowType.Matching, "id2", "name2", "value2")),
                x => x.Given_a_html_report_is_created(),

                x => x.When_a_html_report_is_opened(),
                x => x.Then_the_step_should_have_tabular_parameter_visible_with_rows("step1", "param1",
                    ValueTuple.Create("id1", "name1", "value1"),
                    ValueTuple.Create("id2", "name2", "value2"))
                );
        }
    }
}