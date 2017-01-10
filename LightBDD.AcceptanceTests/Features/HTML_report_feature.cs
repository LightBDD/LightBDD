using LightBDD.Core.Execution.Results;
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
            Runner.Parameterized().RunScenario(
                given => a_various_features_with_scenarios_and_categories(),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
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
            Runner.Parameterized().RunScenario(
                given => a_various_features_with_scenarios_and_categories(),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
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
            Runner.Parameterized().RunScenario(
                given => a_various_features_with_scenarios_and_categories(),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
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
            Runner.Parameterized().RunScenario(
                given => a_various_features_with_scenarios_and_categories(),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
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
        [TestCase(ExecutionStatus.Bypassed)]
        [TestCase(ExecutionStatus.Passed)]
        [TestCase(ExecutionStatus.Failed)]
        [TestCase(ExecutionStatus.Ignored)]
        public void Should_filter_by_status(ExecutionStatus status)
        {
            Runner.Parameterized().RunScenario(
                given => a_various_features_with_scenarios_and_categories(),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_filter_status_button_is_clicked(status),
                then => all_scenarios_with_status_are_VISIBLE(status, false),
                and => all_scenarios_with_status_other_than_STATUS_are_VISIBLE(status, true),
                and => all_features_having_all_scenarios_of_status_are_VISIBLE(status, false),

                when => a_filter_status_button_is_clicked(status),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true)
                );
        }

        [Test]
        [TestCase(ExecutionStatus.Bypassed)]
        [TestCase(ExecutionStatus.Passed)]
        [TestCase(ExecutionStatus.Failed)]
        [TestCase(ExecutionStatus.Ignored)]
        public void Should_filter_by_status_when_there_is_no_categories_filter_bar(ExecutionStatus status)
        {
            Runner.Parameterized().RunScenario(
                given => a_various_features_with_scenarios_but_no_categories(),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_filter_status_button_is_clicked(status),
                then => all_scenarios_with_status_are_VISIBLE(status, false),
                and => all_scenarios_with_status_other_than_STATUS_are_VISIBLE(status, true),
                and => all_features_having_all_scenarios_of_status_are_VISIBLE(status, false),

                when => a_filter_status_button_is_clicked(status),
                and => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true)
                );
        }

        [Test]
        public void Should_filter_by_category()
        {
            Runner.Parameterized().RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA", "catB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                and => a_feature_result("featureB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Passed, "catA"),
                and => a_feature_result("featureC"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                and => a_feature_result("featureD"),
                and => the_feature_has_scenario_result_of_status("featureD", ExecutionStatus.Passed),
                and => a_feature_result("featureE"),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_category_filter_button_is_clicked("catA"),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, true),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),

                and => the_feature_scenario_is_VISIBLE(2, 1, true),
                and => the_feature_is_VISIBLE(3, false),
                and => the_feature_is_VISIBLE(4, false),
                and => the_feature_is_VISIBLE(5, false),

                when => a_category_filter_button_is_clicked("catB"),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, true),

                and => the_feature_is_VISIBLE(2, false),
                and => the_feature_scenario_is_VISIBLE(3, 1, true),
                and => the_feature_is_VISIBLE(4, false),
                and => the_feature_is_VISIBLE(5, false),

                when => a_category_filter_button_is_clicked("-without category-"),
                then => the_feature_is_VISIBLE(1, false),
                and => the_feature_is_VISIBLE(2, false),
                and => the_feature_is_VISIBLE(3, false),
                and => the_feature_scenario_is_VISIBLE(4, 1, true),
                and => the_feature_is_VISIBLE(5, false),

                when => a_category_filter_button_is_clicked("-all-"),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true)
                );
        }

        [Test]
        public void Should_filter_by_category_and_status()
        {
            Runner.Parameterized().RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Failed, "catA", "catB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                and => a_feature_result("featureB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Ignored, "catA"),
                and => a_feature_result("featureC"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => a_category_filter_button_is_clicked("catA"),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, true),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => the_feature_scenario_is_VISIBLE(2, 1, true),
                and => the_feature_scenario_is_VISIBLE(3, 1, true),
                and => the_feature_scenario_is_VISIBLE(3, 2, false),

                when => a_filter_status_button_is_clicked(ExecutionStatus.Passed),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => the_feature_scenario_is_VISIBLE(2, 1, true),
                and => the_feature_scenario_is_VISIBLE(3, 1, false),
                and => the_feature_scenario_is_VISIBLE(3, 2, false),
                and => the_feature_is_VISIBLE(3, false),

                when => a_filter_status_button_is_clicked(ExecutionStatus.Ignored),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => the_feature_is_VISIBLE(2, false),
                and => the_feature_is_VISIBLE(3, false)
                );
        }

        [Test]
        public void Should_follow_shareable_link_with_preserving_selected_options()
        {
            Runner.Parameterized().RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Failed, "catA", "catB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ExecutionStatus.Passed, "catB"),
                and => a_feature_result("featureB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureB", ExecutionStatus.Ignored, "catA"),
                and => a_feature_result("featureC"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ExecutionStatus.Passed, "catB"),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),
                and => the_options_link_is_VISIBLE(false),

                when => a_category_filter_button_is_clicked("catA"),
                and => a_filter_status_button_is_clicked(ExecutionStatus.Passed),
                and => a_filter_status_button_is_clicked(ExecutionStatus.Ignored),
                and => a_scenario_filter_button_is_clicked(),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => the_feature_is_VISIBLE(2, false),
                and => the_feature_is_VISIBLE(3, false),
                and => all_steps_are_VISIBLE(false),
                and => the_options_link_is_VISIBLE(true),

                when => the_options_link_is_clicked(),
                then => the_page_is_redirected_to_url_with_query_part(),
                and => the_category_filter_button_is_SELECTED("catA", true),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Passed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Ignored, false),
                and => the_scenario_filter_button_is_SELECTED(false),

                and => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => the_feature_is_VISIBLE(2, false),
                and => the_feature_is_VISIBLE(3, false)
                );
        }

        [Test]
        public void Should_sort_feature_summary_rows()
        {
            Runner.Parameterized().RunScenario(
                given => a_feature_result("a"),
                and => the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                and => the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                and => the_feature_has_scenario_result_of_status("a", ExecutionStatus.Passed),
                and => a_feature_result("b"),
                and => the_feature_has_scenario_result_of_status("b", ExecutionStatus.Passed),
                and => the_feature_has_scenario_result_of_status("b", ExecutionStatus.Passed),
                and => a_feature_result("c"),
                and => the_feature_has_scenario_result_of_status("c", ExecutionStatus.Passed),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(true, 1),
                when => the_Feature_Summary_table_column_is_clicked(1),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(false, 1),

                when => the_Feature_Summary_table_column_is_clicked(2),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(false, 2),
                when => the_Feature_Summary_table_column_is_clicked(2),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(true, 2),

                when => the_Feature_Summary_table_column_is_clicked(3),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(false, 3),
                when => the_Feature_Summary_table_column_is_clicked(3),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(true, 3),

                when => the_Feature_Summary_table_column_is_clicked(7),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(false, 7),
                when => the_Feature_Summary_table_column_is_clicked(7),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(true, 7),

                when => the_Feature_Summary_table_column_is_clicked(13),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(false, 13),
                when => the_Feature_Summary_table_column_is_clicked(13),
                then => the_Feature_Summary_table_is_sorted_ASCENDING_by_column(true, 13)
                );
        }

        [Test]
        public void Should_show_details_of_non_passed_scenarios()
        {
            Runner.Parameterized().RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Failed),
                and => the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Ignored),
                and => the_feature_has_scenario_result_of_status("featureA", ExecutionStatus.Bypassed),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),

                when => the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Failed),
                then => the_page_is_redirected_to_url_with_query_part(),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Passed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Failed, true),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Ignored, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Bypassed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.NotRun, false),
                and => the_feature_filter_button_is_SELECTED(true),
                and => the_scenario_filter_button_is_SELECTED(false),

                and => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => all_steps_are_VISIBLE(false),

                when => the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Ignored),
                then => the_page_is_redirected_to_url_with_query_part(),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Passed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Failed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Ignored, true),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Bypassed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.NotRun, false),
                and => the_feature_filter_button_is_SELECTED(true),
                and => the_scenario_filter_button_is_SELECTED(false),

                and => the_feature_scenario_is_VISIBLE(1, 1, false),
                and => the_feature_scenario_is_VISIBLE(1, 2, true),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => all_steps_are_VISIBLE(false),

                when => the_link_to_details_of_STATUS_scenarios_is_clicked(ExecutionStatus.Bypassed),
                then => the_page_is_redirected_to_url_with_query_part(),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Passed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Failed, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Ignored, false),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.Bypassed, true),
                and => the_filter_status_button_is_SELECTED(ExecutionStatus.NotRun, false),
                and => the_feature_filter_button_is_SELECTED(true),
                and => the_scenario_filter_button_is_SELECTED(false),

                and => the_feature_scenario_is_VISIBLE(1, 1, false),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, true),
                and => all_steps_are_VISIBLE(false));
        }
    }
}