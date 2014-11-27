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
            Runner.RunScenario(
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
            Runner.RunScenario(
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
            Runner.RunScenario(
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
        [TestCase(ResultStatus.Bypassed)]
        [TestCase(ResultStatus.Passed)]
        [TestCase(ResultStatus.Failed)]
        [TestCase(ResultStatus.Ignored)]
        public void Should_filter_by_status(ResultStatus status)
        {
            Runner.RunScenario(
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
        [TestCase(ResultStatus.Bypassed)]
        [TestCase(ResultStatus.Passed)]
        [TestCase(ResultStatus.Failed)]
        [TestCase(ResultStatus.Ignored)]
        public void Should_filter_by_status_when_there_is_no_categories_filter_bar(ResultStatus status)
        {
            Runner.RunScenario(
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
            Runner.RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catA", "catB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catB"),
                and => a_feature_result("featureB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureB", ResultStatus.Passed, "catA"),
                and => a_feature_result("featureC"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ResultStatus.Passed, "catB"),
                and => a_feature_result("featureD"),
                and => the_feature_has_scenario_result_of_status("featureD", ResultStatus.Passed),
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
            Runner.RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Failed, "catA", "catB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catB"),
                and => a_feature_result("featureB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureB", ResultStatus.Ignored, "catA"),
                and => a_feature_result("featureC"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ResultStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ResultStatus.Passed, "catB"),
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

                when => a_filter_status_button_is_clicked(ResultStatus.Passed),
                then => the_feature_scenario_is_VISIBLE(1, 1, true),
                and => the_feature_scenario_is_VISIBLE(1, 2, false),
                and => the_feature_scenario_is_VISIBLE(1, 3, false),
                and => the_feature_scenario_is_VISIBLE(2, 1, true),
                and => the_feature_scenario_is_VISIBLE(3, 1, false),
                and => the_feature_scenario_is_VISIBLE(3, 2, false),
                and => the_feature_is_VISIBLE(3, false),

                when => a_filter_status_button_is_clicked(ResultStatus.Ignored),
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
            Runner.RunScenario(
                given => a_feature_result("featureA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Failed, "catA", "catB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureA", ResultStatus.Passed, "catB"),
                and => a_feature_result("featureB"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureB", ResultStatus.Ignored, "catA"),
                and => a_feature_result("featureC"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ResultStatus.Passed, "catA"),
                and => the_feature_has_scenario_result_of_status_and_categories("featureC", ResultStatus.Passed, "catB"),
                and => a_html_report_is_created(),

                when => a_html_report_is_opened(),
                then => all_features_are_VISIBLE(true),
                and => all_scenarios_are_VISIBLE(true),
                and => all_steps_are_VISIBLE(true),
                and => the_options_link_is_VISIBLE(false),

                when => a_category_filter_button_is_clicked("catA"),
                and => a_filter_status_button_is_clicked(ResultStatus.Passed),
                and => a_filter_status_button_is_clicked(ResultStatus.Ignored),
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
                and => the_filter_status_button_is_SELECTED(ResultStatus.Passed, false),
                and => the_filter_status_button_is_SELECTED(ResultStatus.Ignored, false),
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
            Runner.RunScenario(
                given => a_feature_result("a"),
                and => the_feature_has_scenario_result_of_status("a", ResultStatus.Passed),
                and => the_feature_has_scenario_result_of_status("a", ResultStatus.Passed),
                and => the_feature_has_scenario_result_of_status("a", ResultStatus.Passed),
                and => a_feature_result("b"),
                and => the_feature_has_scenario_result_of_status("b", ResultStatus.Passed),
                and => the_feature_has_scenario_result_of_status("b", ResultStatus.Passed),
                and => a_feature_result("c"),
                and => the_feature_has_scenario_result_of_status("c", ResultStatus.Passed),
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
    }
}