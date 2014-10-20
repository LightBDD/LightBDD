LightBDD Quick Start:
#####################################

Visual Studio Project Item Templates:
-------------------------------------

Template zip files are included within this package.
Please copy them together with LightBDD [TEST_FRAMEWORK] folder to Visual Studio ItemTemplates folder:

From: packages\LightBDD [VERSION]\Templates
To: [USER_PATH]\Documents\Visual Studio [VERSION]\Templates\ItemTemplates\Visual C#

Acceptance tests creation:
-------------------------------------

Acceptance tests should consist of two files:

1. Acceptance tests definition part containing:
* optional feature description and label,
* list of scenario methods with optional label and body with Runner executing scenario steps 

[TestFixture]
[FeatureDescription(
@"In order to access personal data
As an user
I want to login into system")]
[Label("Story-1")]
public partial class Login_feature
{
	[Test]
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
}

2. Implementation part containing:
* implementation of all step methods
* set up / tear down methods
* inheritance of FeatureFixture base class (it is strongly recommended, however it is possible also to not have it - please use then LightBDD: Standalone Feature Test Class item template, or see Feature Fixture implementation: https://github.com/Suremaker/LightBDD/blob/master/LightBDD/FeatureFixture.cs).

public partial class Login_feature : FeatureFixture
{
	/* scenario data */
	/* set up / tear down methods */

	private void Given_user_is_about_to_login()
	{
		_loginService = new LoginService();
		_loginService.AddUser(_validUserName, _validPassword);
		_loginRequest = new LoginRequest();
	}
	
	private void When_user_clicked_login_button()
	{
		_loginResult = _loginService.Login(_loginRequest);
	}

	private void Then_login_is_successful()
	{
		Assert.That(_loginResult.IsSuccessful, Is.True);
	}

	/* other step definitions */
}

Please note that with usage of provided ItemTemplates, feature class parts would be nicely grouped in Visual Studio.

Since version 1.5.0 it is possible also to define parameterized steps (for details, please follow links from 'More details' section).

Test running:
-------------------------------------
LightBDD integrates with NUnit, MbUnit and MsTest frameworks, so the tests can be run in the same way as regular tests in those frameworks.

During run, the test progress would be printed on console, like:

FEATURE: [Story-1] Login feature
  In order to access personal data
  As an user
  I want to login into system

SCENARIO: [Ticket-1] Successful login
  STEP 1/6: Given user is about to login
  STEP 1/6: Passed after <1ms
  STEP 2/6: Given user entered valid login
  STEP 2/6: Passed after <1ms
  STEP 3/6: Given user entered valid password
  STEP 3/6: Passed after <1ms
  STEP 4/6: When user clicked login button
  STEP 4/6: Passed after <1ms
  STEP 5/6: Then login is successful
  STEP 5/6: Passed after <1ms
  STEP 6/6: Then welcome message is returned containing user name
  STEP 6/6: Passed after 8ms
  SCENARIO RESULT: Passed after 12ms

After all tests are finished, FeaturesSummary.xml would be created in project bin folder with xml summary (it is possible to customize summary file format and location in app.config file) like:
<?xml version="1.0" encoding="utf-8"?>
<TestResults>
  <Summary TestExecutionStart="2014-10-20T20:42:31.2838322Z" TestExecutionTime="PT0.0569615S">
    <Features Count="1" />
    <Scenarios Count="1" Passed="1" Failed="0" Ignored="0" />
    <Steps Count="6" Passed="6" Failed="0" Ignored="0" NotRun="0" />
  </Summary>
  <Feature Name="Login feature" Label="Story-1">
    <Description>In order to access personal data
As an user
I want to login into system</Description>
    <Scenario Status="Passed" Name="Successful login" Label="Ticket-1" ExecutionStart="2014-10-20T20:42:31.2838322Z" ExecutionTime="PT0.0569615S">
      <Step Status="Passed" Number="1" Name="Given user is about to login" ExecutionStart="2014-10-20T20:42:31.2878348Z" ExecutionTime="PT0.0165135S" />
      <Step Status="Passed" Number="2" Name="Given user entered valid login" ExecutionStart="2014-10-20T20:42:31.3238584Z" ExecutionTime="PT0.0003526S" />
      <Step Status="Passed" Number="3" Name="Given user entered valid password" ExecutionStart="2014-10-20T20:42:31.3238584Z" ExecutionTime="PT0.000327S" />
      <Step Status="Passed" Number="4" Name="When user clicked login button" ExecutionStart="2014-10-20T20:42:31.3248595Z" ExecutionTime="PT0.0015506S" />
      <Step Status="Passed" Number="5" Name="Then login is successful" ExecutionStart="2014-10-20T20:42:31.3268604Z" ExecutionTime="PT0.0033607S" />
      <Step Status="Passed" Number="6" Name="Then welcome message is returned containing user name" ExecutionStart="2014-10-20T20:42:31.3308631Z" ExecutionTime="PT0.0088142S" />
    </Scenario>
  </Feature>
</TestResults>

Conventions:
-------------------------------------
* each feature should be represented as separate class with [TestFixture] attribute, where class name is feature name,
* each scenario should be represented as method with [Test] attribute, where method name is feature name,
* scenario method should call BDDRunner class RunScenario method with list of Given/When/Then methods representing steps to execute
* all names should use underscores, that make them a little bit easier to read in code, but they are also transformed to readable text during execution.

More details:
-------------------------------------
For more details, please check:
* project web site: https://github.com/Suremaker/LightBDD
* project wiki: https://github.com/Suremaker/LightBDD/wiki
* example projects: 
  * https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example
  * https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example.AcceptanceTests.NUnit
  * https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example.AcceptanceTests.MbUnit
  * https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example.AcceptanceTests.MsTest