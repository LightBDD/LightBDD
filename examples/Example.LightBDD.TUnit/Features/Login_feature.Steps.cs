using Example.Domain.Services;
using LightBDD.Framework;
using LightBDD.TUnit;
using System.Threading.Tasks;

namespace Example.LightBDD.TUnit.Features
{
    public partial class Login_feature : FeatureFixture
    {
        private const string _validUserName = "admin";
        private const string _validPassword = "password";

        private LoginRequest _loginRequest;
        private LoginService _loginService;
        private LoginResult _loginResult;

        private async Task Given_the_user_is_about_to_login()
        {
            _loginService = new LoginService();
            _loginService.AddUser(_validUserName, _validPassword);
            _loginRequest = new LoginRequest();
        }

        private async Task Given_the_user_entered_valid_login()
        {
            _loginRequest.UserName = _validUserName;
        }

        private async Task Given_the_user_entered_valid_password()
        {
            _loginRequest.Password = _validPassword;
        }

        private async Task Given_the_user_entered_anonymous_login()
        {
            StepExecution.Current.Comment("Presentation of failed scenario");
            _loginRequest.UserName = "anonymous";
        }

        private async Task Given_the_user_entered_invalid_login()
        {
            _loginRequest.UserName = "invalid user";
        }

        private async Task Given_the_user_entered_invalid_password()
        {
            _loginRequest.Password = "invalid password";
        }

        private async Task When_the_user_clicks_login_button()
        {
            _loginResult = _loginService.Login(_loginRequest);
        }

        private async Task Then_the_login_operation_should_be_successful()
        {
            await Assert.That(_loginResult.IsSuccessful).IsTrue();
        }

        private async Task Then_a_welcome_message_containing_user_name_should_be_returned()
        {
            var expectedMessage = string.Format("Welcome {0}!", _validUserName);
            await Assert.That(_loginResult.ResultMessage).IsEqualTo(expectedMessage);
        }

        private async Task Then_the_login_operation_should_be_unsuccessful()
        {
            await Assert.That(_loginResult.IsSuccessful).IsFalse();
        }

        private async Task Then_an_invalid_login_or_password_error_message_should_be_returned()
        {
            await Assert.That(_loginResult.ResultMessage).IsEqualTo("Invalid user name or password.");
        }
    }
}