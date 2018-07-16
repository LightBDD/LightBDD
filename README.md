![logo](https://github.com/LightBDD/LightBDD/blob/master/logo/lightbdd.ico) Lightweight Behavior Driven Development test framework (LightBDD)
===========

Category|Badge |Platforms
--------|------|--------
Build (official) | [![Build status](https://ci.appveyor.com/api/projects/status/xkd7qc950o07o3x8/branch/master?svg=true)](https://ci.appveyor.com/project/Suremaker/lightbdd/branch/master) |
Build (linux/mono) | [![Build Status](https://travis-ci.org/LightBDD/LightBDD.svg?branch=master)](https://travis-ci.org/LightBDD/LightBDD) | 
Chat (gitter) | [![Join the chat at https://gitter.im/LightBDD/LightBDD](https://badges.gitter.im/LightBDD/LightBDD.svg)](https://gitter.im/LightBDD/LightBDD?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) | 
LightBDD.NUnit3 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.NUnit3?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.NUnit3/) | .NET Standard >= 1.6 <br> .NET Framework >= 4.5 <br> .NET Core >= 1.0
LightBDD.NUnit2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.NUnit2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.NUnit2/) | .NET Framework >= 4.5
LightBDD.XUnit2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.XUnit2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.XUnit2/) | .NET Standard >= 1.3 <br> .NET Framework >= 4.5.2 <br> .NET Core >= 1.0
LightBDD.MsTest2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.MsTest2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.MsTest2/) | .NET Standard >= 1.3 <br> .NET Framework >= 4.5 <br> .NET Core >= 1.0 <br> UWP
LightBDD.Fixie2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.Fixie2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.Fixie2/) | .NET Framework >= 4.5.2 <br> .NET Core >= 2.0
LightBDD.Autofac | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.Autofac?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.Autofac/) | .NET Standard >= 1.3 <br> .NET Framework >= 4.5
LightBDD.Extensions.DependencyInjection | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.Extensions.DependencyInjection?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.Extensions.DependencyInjection/) | .NET Standard >= 2.0
LightBDD.MbUnit (1.x) | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.MbUnit?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.MbUnit/) | .NET Framework >= 4.0
LightBDD.MsTest (1.x) | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.MsTest?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.MsTest/) | .NET Framework >= 4.0
LightBDD.NUnit (deprecated) | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.NUnit?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.NUnit/) |
LightBDD.XUnit (deprecated) | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.XUnit?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.XUnit/) |
LightBDD (deprecated) | [![NuGet Badge](https://buildstats.info/nuget/LightBDD?includePreReleases=true)](https://www.nuget.org/packages/LightBDD/) |

Have you ever been using the BDD methodology? Yes? - cool.
Now, have you tried to write stories or acceptance criteria using tools like [SpecFlow](http://www.specflow.org/) or [Fitnesse](http://fitnesse.org/)?
If you have (or have used other similar frameworks) perhaps you came to the point where the test projects contained tens of scenarios or you had multiple projects with a lot of acceptance tests.
You might have also come to the point where you had to modify those tests because the requirements changed, or adjust them to the modified class interfaces after refactoring.
It is no longer fun, is it?

The difficulty with these frameworks is that they are using a totally different language than the one in which the code is written.
Because of that, they try to provide translation layers between the text in which the stories are written and code in which the stories are executed.
This additional layer becomes the root of most problems when maintaining tests, mostly because it lacks integration with development tools, which means that there is no support for refactoring, code analysis (like showing unused methods) etc, or a good integrated environment for running those tests.

## Project description
**LightBDD** is a behaviour-driven development test framework offering ability to write tests that are easy to read, easy to track during execution and summarize in user friendly report, while allowing developers to use all of the standard development tools to maintain them.

### Features
* Native support for refactoring, code analysis (like finding unused methods), test running and all features that Visual Studio / Intellisense / Resharper offer during code development,
* Easy to read scenario definitions,
* Scenario steps execution tracking and time measurement, usable during longer test execution,
* Support for parameterized steps with smart rules of inserting argument values to formatted step name,
* Support for advanced in-line and tabular verifiable parameters,
* Support for contextual scenario execution where steps shares dedicated context,
* Support for DI containers,
* Feature report generation in XML, HTML or Plain text format,
* In-code LightBDD configuration allowing to customize all LightBDD features,
* VisualStudio templates for test classes ([see details](#vs-item-templates)),
* Integrated with [NUnit](http://www.nunit.org/), [xUnit](http://xunit.github.io/), [MsTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework/) and [Fixie](http://fixie.github.io/) frameworks,
* Async scenario and steps execution support,
* Cross-platform support (.NET Framework / .NET Standard / .NET Core / UWP).

### Tests structure and conventions
**LightBDD** is integrated with well known testing frameworks ([NUnit](http://www.nunit.org/), [xUnit](http://xunit.github.io/), [MsTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework/) and [Fixie](http://fixie.github.io/)) which makes it very easy to learn, adapt and use - please see [Tests Structure and Conventions](https://github.com/LightBDD/LightBDD/wiki/Tests-structure-and-conventions) wiki section for details.

### Example 
```C#
[FeatureDescription(
@"In order to access personal data
As an user
I want to login into system")] //feature description
[Label("Story-1")]
public partial class Login_feature //feature name
{
	[Scenario]
	[Label("Ticket-1")]
	public void Successful_login() //scenario name
	{
		Runner.RunScenario(

			Given_the_user_is_about_to_login, //steps
			Given_the_user_entered_valid_login,
			Given_the_user_entered_valid_password,
			When_the_user_clicks_login_button,
			Then_the_login_operation_should_be_successful,
			Then_a_welcome_message_containing_user_name_should_be_returned);
	}
}

[FeatureDescription(
@"In order to pay for products
As a customer
I want to receive invoice for bought items")] //feature description
[Label("Story-2")]
public partial class Invoice_feature //feature name
{
	[Scenario]
	[Label("Ticket-2")]
	public void Receiving_invoice_for_products() //scenario name
	{
		Runner.RunScenario(

			_ => Given_product_is_available_in_product_storage("wooden desk"), //steps
			_ => Given_product_is_available_in_product_storage("wooden shelf"),
			_ => When_customer_buys_product("wooden desk"),
			_ => When_customer_buys_product("wooden shelf"),
			_ => Then_an_invoice_should_be_sent_to_the_customer(),
			_ => Then_the_invoice_should_contain_product_with_price_of_AMOUNT("wooden desk", 62),
			_ => Then_the_invoice_should_contain_product_with_price_of_AMOUNT("wooden shelf", 37));
	}
}
```
The above example shows feature *partial* classes containing only scenario definitions, which makes it easy to read.
The `Login_feature` class uses basic scenario format for defining scenario steps.
The `Invoice_feature` class uses extended scenario format allowing to use parameterized steps.

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
	private void Given_product_is_available_in_product_storage(string product) { /* ... */ }

	private void When_customer_buys_product(string product) { /* ... */ }

	private void Then_an_invoice_should_be_sent_to_the_customer() { /* ... */ }

	private void Then_the_invoice_should_contain_product_with_price_of_AMOUNT(string product, int amount)
	{ /* ... */ }
	/* ... */
}
```
With partial classes, it is possible to keep all methods describing steps as **private**, which makes scenarios easier to read and allows tools like Resharper to mark them if they are no longer used, making maintenance easier.

### Example console output during tests execution:
```
FEATURE: [Story-1] Login feature
  In order to access personal data
  As an user
  I want to login into system

SCENARIO: [Ticket-1] Successful login
  STEP 1/6: GIVEN the user is about to login...
  STEP 1/6: GIVEN the user is about to login (Passed after 2ms)
  STEP 2/6: AND the user entered valid login...
  STEP 2/6: AND the user entered valid login (Passed after <1ms)
  STEP 3/6: AND the user entered valid password...
  STEP 3/6: AND the user entered valid password (Passed after <1ms)
  STEP 4/6: WHEN the user clicks login button...
  STEP 4/6: WHEN the user clicks login button (Passed after 125ms)
  STEP 5/6: THEN the login operation should be successful...
  STEP 5/6: THEN the login operation should be successful (Passed after 4ms)
  STEP 6/6: AND a welcome message containing user name should be returned...
  STEP 6/6: AND a welcome message containing user name should be returned (Passed after 9ms)
  SCENARIO RESULT: Passed after 164ms


FEATURE: [Story-2] Invoice feature
  In order to pay for products
  As a customer
  I want to receive invoice for bought items

SCENARIO: [Ticket-4] Receiving invoice for products
  STEP 1/7: GIVEN product "wooden desk" is available in product storage...
  STEP 1/7: GIVEN product "wooden desk" is available in product storage (Passed after 2ms)
  STEP 2/7: AND product "wooden shelf" is available in product storage...
  STEP 2/7: AND product "wooden shelf" is available in product storage (Passed after <1ms)
  STEP 3/7: WHEN customer buys product "wooden desk"...
  STEP 3/7: WHEN customer buys product "wooden desk" (Passed after <1ms)
  STEP 4/7: AND customer buys product "wooden shelf"...
  STEP 4/7: AND customer buys product "wooden shelf" (Passed after <1ms)
  STEP 5/7: THEN an invoice should be sent to the customer...
  STEP 5/7: THEN an invoice should be sent to the customer (Passed after <1ms)
  STEP 6/7: AND the invoice should contain product "wooden desk" with price of "£62.00"...
  STEP 6/7: AND the invoice should contain product "wooden desk" with price of "£62.00" (Passed after <1ms)
  STEP 7/7: AND the invoice should contain product "wooden shelf" with price of "£37.00"...
  STEP 7/7: AND the invoice should contain product "wooden shelf" with price of "£37.00" (Passed after <1ms)
  SCENARIO RESULT: Passed after 30ms
```

### Example HTML report generated after test execution:

![](https://raw.githubusercontent.com/wiki/LightBDD/LightBDD/images/scenario_report.png)

To read more, please see [Generating Reports](https://github.com/LightBDD/LightBDD/wiki/Generating-Reports) wiki page.

## Installation
The LightBDD is available on [NuGet](https://www.nuget.org/packages?q=LightBDD): 

* `PM> Install-Package LightBDD.NUnit3` for package using NUnit 3x
* `PM> Install-Package LightBDD.NUnit2` for package using NUnit 2x
* `PM> Install-Package LightBDD.XUnit2` for package using xUnit 2x
* `PM> Install-Package LightBDD.MsTest2` for package using MsTest.TestFramework
* `PM> Install-Package LightBDD.Fixie2` for package using Fixie 2x

## VS Item Templates
**LightBDD** project offers also set of templates that can be installed in Visual Studio.
They can be installed via [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/99be2036-a6b3-462c-9111-3ca31a317da2) (Visual Studio 2012 and newer versions supported)

## Wiki
Please check project [wiki](https://github.com/LightBDD/LightBDD/wiki) for more details.

## Debugging LightBDD from NuGet packages

The nuget packages for LightBDD are being pushed together with symbol packages.
[The easy way to publish NuGet packages with sources](http://blog.davidebbo.com/2011/04/easy-way-to-publish-nuget-packages-with.html) article describes how to enable debugging with symbols downloaded from **SymbolSource.org**. Please check *What the package Consumer needs to do* article section for details.

In short, it is needed to do two actions in Visual Studio:
* go to TOOLS->Options->Debugging->General and uncheck 'Enable Just My Code',
* go to TOOLS->Options->Debugging->General and check 'Enable source server support',
* go to TOOLS->Options->Debugging->Symbols and add 'http://srv.symbolsource.org/pdb/Public' as a symbol location

More information on: [http://www.symbolsource.org/Public/Home/VisualStudio](http://www.symbolsource.org/Public/Home/VisualStudio).

## More information about LightBDD

If you are interested about background of LightBDD creation or getting insight into what is driving it's evolution, please feel free to take a look at my [blog posts](http://woitech.eu/blog/category/lightbdd/).
