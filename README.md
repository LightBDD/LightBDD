![logo](https://github.com/Suremaker/LightBDD/blob/master/logo/lightbdd.ico) Lightweight Behavior Driven Development test framework (LightBDD)
===========

Have you ever been using the BDD methodology? Yes? - cool.
Now, have you tried to write stories or acceptance criteria using tools like [SpecFlow](http://www.specflow.org/) or [Fitnesse](http://fitnesse.org/)?
If you have (or have used other similar frameworks) perhaps you came to the point where the test projects contained tens of scenarios or you had multiple projects with a lot of acceptance tests.
You might have also come to the point where you had to modify those tests because the requirements changed, or adjust them to the modified class interfaces after refactoring.
It is no longer fun, is it?

The difficulty with these frameworks is that they are using a totally different language than the one in which the code is written.
Because of that, they try to provide translation layers between the text in which the stories are written and code in which the stories are executed.
This additional layer becomes the root of most problems when maintaining tests, mostly because it lacks integration with development tools, which means that there is no support for refactoring, code analysis (like showing unused methods) etc, or a good integrated environment for running those tests.

## Project description
**The purpose of this project** is to provide framework which would be as close to the development environment as possible (so developers would be able to use all of the standard development tools to maintain it), but also offering easy to read tests by people who are not experts in writing code, easier to track during longer execution, and easy to summarize.

### Features
* Native support for refactoring, code analysis (like finding unused methods), test running and all features that Visual Studio / Intellisense / Resharper offer during code development,
* Easy to read scenario definitions,
* Scenario steps execution tracking, usable during longer test execution,
* Scenario steps execution time measurement,
* Possibility to run steps with dedicated shared context, allowing to run tests safely in parallel,
* Possibility to run parameterized steps with smart rules of inserting argument values to formatted step name,
* Feature result summary available in XML, HTML or Plain text format,
* Possibility to configure multiple result summaries in app.config file,
* VS Project Item templates for feature test files,
* Integrated with [NUnit](http://www.nunit.org/), [MbUnit](https://code.google.com/p/mb-unit/) and [MsTest](http://msdn.microsoft.com/en-us/library/ms243147) frameworks.

### Tests structure and conventions
**LightBDD** is integrated with well known testing frameworks ([NUnit](http://www.nunit.org/), [MbUnit](https://code.google.com/p/mb-unit/) and [MsTest](http://msdn.microsoft.com/en-us/library/ms243147)) which makes it very easy to learn, adapt and use - please see [Tests Structure and Conventions](https://github.com/Suremaker/LightBDD/wiki/02-Tests-structure-and-conventions) wiki section for details.

### Example 
```C#
[TestFixture]
[FeatureDescription(
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

			Given_user_is_about_to_login, //steps
			Given_user_entered_valid_login,
			Given_user_entered_valid_password,
			When_user_clicked_login_button,
			Then_login_is_successful,
			Then_welcome_message_is_returned_containing_user_name);
	}
}

[FeatureDescription(
@"In order to pay for products
As a customer
I want to receive invoice for bought items")] //feature description
[TestFixture]
[Label("Story-2")]
public partial class Invoice_feature
{
	[Test]
	[Label("Ticket-2")]
	public void Receiving_invoice_for_products() //scenario name
	{
		Runner.RunScenario(

			given => Product_is_available_in_products_storage("wooden desk"), //steps
			and => Product_is_available_in_products_storage("wooden shelf"),
			when => Customer_buys_product("wooden desk"),
			and => Customer_buys_product("wooden shelf"),
			then => Invoice_is_sent_to_customer(),
			and => Invoice_contains_product_with_price_of_AMOUNT_pounds("wooden desk", 62),
			and => Invoice_contains_product_with_price_of_AMOUNT_pounds("wooden shelf", 37));
	}
}
```
The above example shows feature *partial* classes containing only scenario definitions, which makes it easy to read.
The **Login_feature** class uses simplified syntax for defining scenario steps.
The **Invoice_feature** class uses extended syntax that separates action type (given, when, then) from step method and allows to use parameterized steps.

All method implementations are separated and put in other files.
```C#
public partial class Login_feature : FeatureFixture
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

public partial class Invoice_feature : FeatureFixture
{
	private void Product_is_available_in_products_storage(string product) { /* ... */ }

	private void Customer_buys_product(string product) { /* ... */ }

	private void Invoice_is_sent_to_customer() { /* ... */ }

	private void Invoice_contains_product_with_price_of_AMOUNT_pounds(string product, int amount) { /* ... */ }
	/* ... */
}
```
With using partial classes, it is possible to keep all methods describing steps as **private**, which allows Resharper to mark them if they are no longer used.

### Example console output during tests execution:
```
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
  SCENARIO RESULT: Passed after 13ms

FEATURE: [Story-2] Invoice feature
  In order to pay for products
  As a customer
  I want to receive invoice for bought items

SCENARIO: [Ticket-2] Receiving invoice for products
  STEP 1/7: GIVEN Product "wooden desk" is available in products storage
  STEP 1/7: Passed after 128ms
  STEP 2/7: AND Product "wooden shelf" is available in products storage
  STEP 2/7: Passed after 967ms
  STEP 3/7: WHEN Customer buys product "wooden desk"
  STEP 3/7: Passed after 443ms
  STEP 4/7: AND Customer buys product "wooden shelf"
  STEP 4/7: Passed after 671ms
  STEP 5/7: THEN Invoice is sent to customer
  STEP 5/7: Passed after 258ms
  STEP 6/7: AND Invoice contains product "wooden desk" with price of "62" pounds
  STEP 6/7: Passed after <1ms
  STEP 7/7: AND Invoice contains product "wooden shelf" with price of "37" pounds
  STEP 7/7: Passed after <1ms
  SCENARIO RESULT: Passed after 2s 484ms
```

### Example summary output file after all tests execution:
```xml
<?xml version="1.0" encoding="utf-8"?>
<TestResults>
  <Summary TestExecutionStart="2014-10-20T21:07:26.9080605Z" TestExecutionTime="PT0.061447S">
    <Features Count="2" />
    <Scenarios Count="2" Passed="2" Failed="0" Ignored="0" />
    <Steps Count="13" Passed="13" Failed="0" Ignored="0" NotRun="0" />
  </Summary>
  <Feature Name="Invoice feature" Label="Story-2">
    <Description>In order to pay for products
As a customer
I want to receive invoice for bought items</Description>
    <Scenario Status="Passed" Name="Receiving invoice for products" Label="Ticket-4" ExecutionStart="2014-10-20T21:07:26.9080605Z" ExecutionTime="PT0.0226216S">
      <Step Status="Passed" Number="1" Name="GIVEN Product &quot;wooden desk&quot; is available in products storage" ExecutionStart="2014-10-20T21:07:26.9130638Z" ExecutionTime="PT0.0001623S" />
      <Step Status="Passed" Number="2" Name="AND Product &quot;wooden shelf&quot; is available in products storage" ExecutionStart="2014-10-20T21:07:26.9270732Z" ExecutionTime="PT0.0000023S" />
      <Step Status="Passed" Number="3" Name="WHEN Customer buys product &quot;wooden desk&quot;" ExecutionStart="2014-10-20T21:07:26.9280734Z" ExecutionTime="PT0.0001609S" />
      <Step Status="Passed" Number="4" Name="AND Customer buys product &quot;wooden shelf&quot;" ExecutionStart="2014-10-20T21:07:26.9280734Z" ExecutionTime="PT0.0000032S" />
      <Step Status="Passed" Number="5" Name="THEN Invoice is sent to customer" ExecutionStart="2014-10-20T21:07:26.9290745Z" ExecutionTime="PT0.0001203S" />
      <Step Status="Passed" Number="6" Name="AND Invoice contains product &quot;wooden desk&quot; with price of &quot;62&quot; pounds" ExecutionStart="2014-10-20T21:07:26.9290745Z" ExecutionTime="PT0.0001292S" />
      <Step Status="Passed" Number="7" Name="AND Invoice contains product &quot;wooden shelf&quot; with price of &quot;37&quot; pounds" ExecutionStart="2014-10-20T21:07:26.9300761Z" ExecutionTime="PT0.0000027S" />
    </Scenario>
  </Feature>
  <Feature Name="Login feature" Label="Story-1">
    <Description>In order to access personal data
As an user
I want to login into system</Description>
    <Scenario Status="Passed" Name="Successful login" Label="Ticket-1" ExecutionStart="2014-10-20T21:07:27.1092638Z" ExecutionTime="PT0.0388254S">
      <Step Status="Passed" Number="1" Name="Given user is about to login" ExecutionStart="2014-10-20T21:07:27.1132656Z" ExecutionTime="PT0.0167375S" />
      <Step Status="Passed" Number="2" Name="Given user entered valid login" ExecutionStart="2014-10-20T21:07:27.1302779Z" ExecutionTime="PT0.0003176S" />
      <Step Status="Passed" Number="3" Name="Given user entered valid password" ExecutionStart="2014-10-20T21:07:27.1312767Z" ExecutionTime="PT0.0003004S" />
      <Step Status="Passed" Number="4" Name="When user clicked login button" ExecutionStart="2014-10-20T21:07:27.1322773Z" ExecutionTime="PT0.0014676S" />
      <Step Status="Passed" Number="5" Name="Then login is successful" ExecutionStart="2014-10-20T21:07:27.1342801Z" ExecutionTime="PT0.0030612S" />
      <Step Status="Passed" Number="6" Name="Then welcome message is returned containing user name" ExecutionStart="2014-10-20T21:07:27.1382823Z" ExecutionTime="PT0.0095448S" />
    </Scenario>
  </Feature>
</TestResults>
```

For full example, please see corresponding example projects:
* https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example
* https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example.AcceptanceTests.NUnit
* https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example.AcceptanceTests.MbUnit
* https://github.com/Suremaker/LightBDD/tree/master/LightBDD.Example.AcceptanceTests.MsTest

## Download
It is possible to download package using [NuGet](http://nuget.org):  
`PM> Install-Package LightBDD` for main package using NUnit  
`PM> Install-Package LightBDD.MbUnit` for package using MbUnit  
or to clone sources from git: `git clone git://github.com/Suremaker/LightBDD.git`

## VS Project Item Templates
**LightBDD** project offers also set of templates that can be installed in Visual Studio.
Please check [Templates](https://github.com/Suremaker/LightBDD/tree/master/Templates) folder for details how to install them.

## Limitations
In order to display scenario names properly, the project containing feature classes has to be compiled in **Debug** mode or with **[assembly: Debuggable(true, true)]** attribute or scenario methods have to have **[MethodImpl(MethodImplOptions.NoInlining)]** attribute applied, or **BDDRunner.RunScenarios()** has to be called with explicit scenario name.

## Wiki
Please check project [wiki](https://github.com/Suremaker/LightBDD/wiki) for more details.

## Debugging LightBDD from NuGet packages

Since 1.4.0.0 version, nuget packages for LightBDD are being pushed together with symbol packages.
[The easy way to publish NuGet packages with sources](http://blog.davidebbo.com/2011/04/easy-way-to-publish-nuget-packages-with.html) article describes how to enable debugging with symbols downloaded from **SymbolSource.org**. Please check *What the package Consumer needs to do* article section for details.
