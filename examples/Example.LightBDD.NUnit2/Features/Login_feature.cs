using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.NUnit2;

namespace Example.LightBDD.NUnit2.Features
{
    [FeatureDescription(
@"In order to access personal data
As an user
I want to login into system")]
    [Label("Story-1")]
    public partial class Login_feature
    {
        [Scenario]
        [Label("Ticket-1")]
        [ScenarioCategory(Categories.Security)]
        public void Successful_login()
        {
            Runner.RunScenario(

                Given_the_user_is_about_to_login,
                Given_the_user_entered_valid_login,
                Given_the_user_entered_valid_password,
                When_the_user_clicks_login_button,
                Then_the_login_operation_should_be_successful,
                Then_a_welcome_message_containing_user_name_should_be_returned);
        }

        [Scenario]
        [Label("Ticket-2")]
        [ScenarioCategory(Categories.Security)]
        public void Wrong_login_provided_causes_login_to_fail()
        {
            Runner.RunScenario(

                Given_the_user_is_about_to_login,
                Given_the_user_entered_invalid_login,
                Given_the_user_entered_valid_password,
                When_the_user_clicks_login_button,
                Then_the_login_operation_should_be_unsuccessful,
                Then_an_invalid_login_or_password_error_message_should_be_returned);
        }

        [Scenario]
        [Label("Ticket-2")]
        [ScenarioCategory(Categories.Security)]
        public void Wrong_password_provided_causes_login_to_fail()
        {
            Runner.RunScenario(

                Given_the_user_is_about_to_login,
                Given_the_user_entered_valid_login,
                Given_the_user_entered_invalid_password,
                When_the_user_clicks_login_button,
                Then_the_login_operation_should_be_unsuccessful,
                Then_an_invalid_login_or_password_error_message_should_be_returned);
        }

        /// <summary>
        /// This test presents how LightBDD treats test failures
        /// </summary>
        [Scenario]
        [Label("Ticket-3")]
        [ScenarioCategory(Categories.Security)]
        public void Anonymous_login_name_should_allow_to_log_in()
        {
            Runner.RunScenario(

                Given_the_user_is_about_to_login,
                Given_the_user_entered_anonymous_login,
                When_the_user_clicks_login_button,
                Then_the_login_operation_should_be_successful,
                Then_a_welcome_message_containing_user_name_should_be_returned);
        }
    }
}