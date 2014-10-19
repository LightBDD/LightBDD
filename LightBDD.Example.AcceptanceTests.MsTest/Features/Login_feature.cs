using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Example.AcceptanceTests.MsTest.Features
{
    [TestClass]
    [FeatureDescription(
@"In order to access personal data
As an user
I want to login into system")]
    [Label("Story-1")]
    public partial class Login_feature
    {
        [TestMethod]
        [Label("Ticket-1")]
        public void Successful_login()
        {
            Runner.RunScenario(

                Given_user_is_about_to_login,
                Given_user_entered_valid_login,
                Given_user_entered_valid_password,
                When_user_clicked_login_button,
                Then_login_is_successful,
                Then_welcome_message_is_returned_containing_user_name);
        }

        [TestMethod]
        [Label("Ticket-2")]
        public void Wrong_login_provided_causes_login_to_fail()
        {
            Runner.RunScenario(

                Given_user_is_about_to_login,
                Given_user_entered_invalid_login,
                Given_user_entered_valid_password,
                When_user_clicked_login_button,
                Then_login_is_unsuccessful,
                Then_invalid_login_or_password_error_message_is_returned);
        }

        [TestMethod]
        [Label("Ticket-2")]
        public void Wrong_password_provided_causes_login_to_fail()
        {
            Runner.RunScenario(

                Given_user_is_about_to_login,
                Given_user_entered_valid_login,
                Given_user_entered_invalid_password,
                When_user_clicked_login_button,
                Then_login_is_unsuccessful,
                Then_invalid_login_or_password_error_message_is_returned);
        }

        /// <summary>
        /// This test presents how LightBDD treats test failures
        /// </summary>
        [TestMethod]
        [Label("Ticket-3")]
        public void Anonymous_login_name_should_allow_to_log_in()
        {
            Runner.RunScenario(

                Given_user_is_about_to_login,
                Given_user_entered_anonymous_login,
                When_user_clicked_login_button,
                Then_login_is_successful,
                Then_welcome_message_is_returned_containing_user_name);
        }
    }
}