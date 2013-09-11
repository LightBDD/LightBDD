// ReSharper disable CheckNamespace

using System;
using NUnit.Framework;
using SimpleBDD.Example.Services;
using SimpleBDD.Results.Formatters;

namespace SimpleBDD.Example
{
	public partial class LoginFeature
	{
		private const string _validUserName = "admin";
		private const string _validPassword = "password";

		private BDDRunner _bddRunner;
		private LoginRequest _loginRequest;
		private LoginService _loginService;
		private LoginResult _loginResult;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_bddRunner = new BDDRunner();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Console.WriteLine(new XmlResultFormatter().Format(_bddRunner.StoryResult));
			Console.WriteLine(new PlainTextResultFormatter().Format(_bddRunner.StoryResult));
		}

		private void Given_user_is_about_to_login()
		{
			_loginService = new LoginService();
			_loginService.AddUser(_validUserName, _validPassword);
			_loginRequest = new LoginRequest();
		}

		private void Given_user_entered_valid_login()
		{
			_loginRequest.UserName = _validUserName;
		}

		private void Given_user_entered_valid_password()
		{
			_loginRequest.Password = _validPassword;
		}

		private void When_user_clicked_login_button()
		{
			_loginResult = _loginService.Login(_loginRequest);
		}

		private void Then_login_is_successful()
		{
			Assert.That(_loginResult.IsSuccessful, Is.True);
		}

		private void Then_welcome_message_is_returned_containing_user_name()
		{
			var expectedMessage = string.Format("Welcome {0}!", _validUserName);
			Assert.That(_loginResult.ResultMessage, Is.EqualTo(expectedMessage));
		}

		private void Given_user_entered_invalid_login()
		{
			_loginRequest.UserName = "invalid user";
		}

		private void Then_login_is_unsuccessful()
		{
			Assert.That(_loginResult.IsSuccessful, Is.False);
		}

		private void Then_invalid_login_or_password_error_message_is_returned()
		{
			Assert.That(_loginResult.ResultMessage, Is.EqualTo("Invalid user name or password."));
		}

		private void Given_user_entered_invalid_password()
		{
			_loginRequest.Password = "invalid password";
		}
	}
}
// ReSharper restore CheckNamespace