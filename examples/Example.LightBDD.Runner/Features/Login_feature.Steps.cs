using Example.Domain.Services;
using LightBDD.Framework;
using LightBDD.XUnit2;
using Xunit;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Login_feature : FeatureFixture
    {
        private const string _validUserName = "admin";
        private const string _validPassword = "password";

        private LoginRequest _loginRequest;
        private LoginService _loginService;
        private LoginResult _loginResult;
        private void Given_the_user_is_about_to_login()
        {
            _loginService = new LoginService();
            _loginService.AddUser(_validUserName, _validPassword);
            _loginRequest = new LoginRequest();
        }

        private void Given_the_user_entered_valid_login()
        {
            _loginRequest.UserName = _validUserName;
        }

        private void Given_the_user_entered_valid_password()
        {
            _loginRequest.Password = _validPassword;
        }

        private void Given_the_user_entered_anonymous_login()
        {
            StepExecution.Current.Comment("Presentation of failed scenario");
            _loginRequest.UserName = "anonymous";
        }

        private void Given_the_user_entered_invalid_login()
        {
            _loginRequest.UserName = "invalid user";
        }

        private void Given_the_user_entered_invalid_password()
        {
            _loginRequest.Password = "invalid password";
        }

        private void When_the_user_clicks_login_button()
        {
            _loginResult = _loginService.Login(_loginRequest);
        }

        private void Then_the_login_operation_should_be_successful()
        {
            Assert.True(_loginResult.IsSuccessful, "Login should succeeded");
        }

        private void Then_a_welcome_message_containing_user_name_should_be_returned()
        {
            var expectedMessage = string.Format("Welcome {0}!", _validUserName);
            Assert.Equal(expectedMessage, _loginResult.ResultMessage);
        }

        private void Then_the_login_operation_should_be_unsuccessful()
        {
            Assert.False(_loginResult.IsSuccessful);
        }

        private void Then_an_invalid_login_or_password_error_message_should_be_returned()
        {
            Assert.Equal("Invalid user name or password.", _loginResult.ResultMessage);
        }
    }
}