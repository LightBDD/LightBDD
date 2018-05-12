using LightBDD.Fixie2;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;

namespace Example.LightBDD.Fixie2.Features
{
    /// <summary>
    /// This feature class presents usage of basic scenario syntax (that is most readable)
    /// as well as attributes and concepts of creating LightBDD tests.
    /// 
    /// More information on basic scenario syntax can be found here: https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#basic-scenarios
    /// The attributes used here are described on: https://github.com/LightBDD/LightBDD/wiki/Tests-Structure-and-Conventions
    /// </summary>
    [FeatureDescription(
@"In order to access personal data
As an user
I want to login into system")]
    [Label("Story-1")]
    public partial class Login_feature
    {
        /// <summary>
        /// This is a sample scenario.
        /// When executed, it would appear in the report as "Successful login" and it would consists of following steps:
        /// 
        /// GIVEN the user is about to login
        /// GIVEN the user entered valid login
        /// GIVEN the user entered valid password
        /// WHEN the user clicks login button
        /// THEN the login operation should be successful
        /// THEN a welcome message containing user name should be returned
        /// </summary>
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