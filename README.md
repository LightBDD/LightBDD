Lightweight Behavior Driven Development test framework.
===========

Have you ever been using the BDD methodology? Yes? - cool.  
Now, have you tried to write stories or acceptance criteria using tools like [SpecFlow](http://www.specflow.org/) or [Fitnesse](http://fitnesse.org/)?  
If you have (or have used other similar frameworks) perhaps you came to the point where the test projects contained tens of scenarios or you had multiple projects with a lot of acceptance tests.  
You might have also come to the point where you had to modify those tests because the requirements changed, or adjust them to the modified class interfaces after refactoring.  
It is no longer fun, is it?

The difficulty with these frameworks is that they are using a totally different language than the one in which the code is written.  
Because of that, they try to provide translation layers between the text in which the stories are written and code in which the stories are executed.  
This additional layer is becomes the root of most problems when maintaining tests, mostly because it lacks integration with development tools, which means that there is no support for refactoring, code analysis (like showing unused methods) etc, or a good integrated environment for running those tests.

## Project description
**The purpose of this project** is to provide framework which would be as close to the development environment as possible (so developers would be able to use all of the standard development tools to maintain it), but also offering easy to read tests by people who are not experts in writing code, easier to track during longer execution, and easy to summarize.

### Features
* Native support for refactoring, code analysis (like finding unused methods), test running and all features that Visual Studio / Intellisense / Resharper offer during code development,
* Easy to read scenario definitions,
* Scenario steps execution tracking, usable during longer test execution,
* Feature result summary available in XML or Plain text format,
* VS Project Item templates for feature test files.

### Tests structure and conventions
**LightBDD** is based on the [NUnit](http://www.nunit.org/) framework which makes it very easy to learn and use - please see [Tests structure and conventions](https://github.com/Suremaker/LightBDD/wiki/Tests-structure-and-conventions) wiki section for details.

### Example 
```C#
[TestFixture]
[Description(
@"In order to access personal data
As an user
I want to login into system")] //feature description
[Label("Story-1")]
public partial class Login_feature //feature name
{
	[Test]
	[Label("Ticket-1")]
	public void Successful_login() //scenario name
	{
		Runner.RunScenario(

			Given_user_is_about_to_login, //scenario steps
			Given_user_entered_valid_login,
			Given_user_entered_valid_password,
			When_user_clicked_login_button,
			Then_login_is_successful,
			Then_welcome_message_is_returned_containing_user_name);
	}
}
```
The above example shows feature *partial* class containing only scenario definitions, which makes it easy to read.

All method implementations are separated and put in other file.
```C#
public partial class Login_feature : FeatureTestsBase
{
	private const string _validUserName = "admin";
	private const string _validPassword = "password";

	private LoginRequest _loginRequest;
	private LoginService _loginService;
	private LoginResult _loginResult;

	private void Given_user_is_about_to_login()
	{
		_loginService = new LoginService();
		_loginService.AddUser(_validUserName, _validPassword);
		_loginRequest = new LoginRequest();
	}
	/* ... */	
}
```
With using partial classes, it is possible to keep all methods describing steps as **private**, which allows Resharper to mark them if they are no longer used.

### Example console output during tests execution:
```
SCENARIO: [Ticket-1] Successful login
  STEP 1/6: Given user is about to login
  STEP 2/6: Given user entered valid login
  STEP 3/6: Given user entered valid password
  STEP 4/6: When user clicked login button
  STEP 5/6: Then login is successful
  STEP 6/6: Then welcome message is returned containing user name
  SCENARIO RESULT: Passed
```

### Example summary output file after all tests execution:
```xml
<?xml version="1.0" encoding="utf-8"?>
<TestResults>
  <Feature Name="Login feature" Label="Story-1">
    <Description>In order to access personal data
As an user
I want to login into system</Description>
    <Scenario Status="Passed" Name="Successful login" Label="Ticket-1">
      <Step Status="Passed" Number="1" Name="Given user is about to login" />
      <Step Status="Passed" Number="2" Name="Given user entered valid login" />
      <Step Status="Passed" Number="3" Name="Given user entered valid password" />
      <Step Status="Passed" Number="4" Name="When user clicked login button" />
      <Step Status="Passed" Number="5" Name="Then login is successful" />
      <Step Status="Passed" Number="6" Name="Then welcome message is returned containing user name" />
    </Scenario>
  </Feature>
</TestResults>
```

For full example, please see [LightBDD.Example](https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example) project.

## Download
It is possible to download package using [NuGet](http://nuget.org): `PM> Install-Package LightBDD`   
or to clone sources from git: `git clone git://github.com/Suremaker/Spring.FluentContext.git`

## VS Project Item Templates
**LightBDD** project offers also set of templates that can be installed in Visual Studio.
Please check [Templates](https://github.com/Suremaker/LightBDD/tree/master/Templates) folder for details how to install them.

## Limitations
In order to display scenario names properly, the project containing feature classes has to be compiled in **Debug** mode or the scenario method has to have **[MethodImpl(MethodImplOptions.NoInlining)]** attribute, or **BDDRunner.RunScenarios()** with has to be called with explicit scenario name.

## Wiki
Please check project [wiki](https://github.com/Suremaker/LightBDD/wiki) for more details.
