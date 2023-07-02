![logo](https://github.com/LightBDD/LightBDD/blob/master/logo/lightbdd.ico) LightBDD <br><br>The Lightweight Behavior Driven Development test framework
===========

Category|Badge
--------|------
Build | [![Build status](https://ci.appveyor.com/api/projects/status/xkd7qc950o07o3x8/branch/master?svg=true)](https://ci.appveyor.com/project/Suremaker/lightbdd/branch/master)
Chat (gitter) | [![Join the chat at https://gitter.im/LightBDD/LightBDD](https://badges.gitter.im/LightBDD/LightBDD.svg)](https://gitter.im/LightBDD/LightBDD?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
LightBDD.NUnit3 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.NUnit3?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.NUnit3/)
LightBDD.XUnit2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.XUnit2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.XUnit2/)
LightBDD.MsTest2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.MsTest2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.MsTest2/)
LightBDD.Fixie2 | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.Fixie2?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.Fixie2/)
LightBDD.Autofac | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.Autofac?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.Autofac/)
LightBDD.Extensions.DependencyInjection | [![NuGet Badge](https://buildstats.info/nuget/LightBDD.Extensions.DependencyInjection?includePreReleases=true)](https://www.nuget.org/packages/LightBDD.Extensions.DependencyInjection/)

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
* Feature report generation in HTML, XML or Plain text format,
* In-code LightBDD configuration allowing to customize all LightBDD features,
* Productivity extensions for VisualStudio with Feature Class Templates, Project Templates and Code Snippets,
* Integration with [NUnit](http://www.nunit.org/), [xUnit](http://xunit.github.io/), [MsTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework/) and [Fixie](http://fixie.github.io/) frameworks,
* Async scenario and steps execution support,
* Cross-platform support (.NET 5+ / .NET Framework / .NET Standard / .NET Core / [UWP](https://github.com/LightBDD/LightBDD/tree/3.4.2/examples/Example.LightBDD.MsTest2.UWP)).

### Tests structure and conventions
**LightBDD** runs on top of [NUnit](http://www.nunit.org/), [xUnit](http://xunit.github.io/), [MsTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework/) and [Fixie](http://fixie.github.io/), allowing to leverage the well known test frameworks and their features in writing the behavioral style scenarios, which makes it very easy to learn, adapt and use.

To learn more, please see [LightBDD wiki page](https://github.com/LightBDD/LightBDD/wiki), or jump straight to:
* [Quick Start](https://github.com/LightBDD/LightBDD/wiki/Quick-Start) and followup documentation pages,
* [What Is New](https://github.com/LightBDD/LightBDD/wiki/What-Is-New) section with the newest features introduced in the library,
* [Examples](https://github.com/LightBDD/LightBDD/tree/master/examples) demonstrating features and integrations of LightBDD,
* [Tutorials](https://github.com/LightBDD/LightBDD.Tutorials) with complete sample projects using LightBDD features.

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

The above example shows feature *partial* classes containing scenario definitions only, which makes it easier to read.  
The `Login_feature` class uses [basic scenario format](https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#basic-scenarios) for defining scenario steps.  
The `Invoice_feature` class uses [extended scenario format](https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition#extended-scenarios) allowing to use parameterized steps.  
LightBDD offers multiple scenario formats and flavors suitable for various use cases - for details, please check [Scenario Steps Definition](https://github.com/LightBDD/LightBDD/wiki/Scenario-Steps-Definition) page.


The implementation of step methods is located in other part of the class, in separate file and leverages standard features of the underlying test framwork (such as assert mechanisms) empowered by LightBDD features such as:
* [Dependency Injection](https://github.com/LightBDD/LightBDD/wiki/DI-Containers),
* [Inline, Tabular, Tree-hierarchy parameters](https://github.com/LightBDD/LightBDD/wiki/Advanced-Step-Parameters),
* [Expectation Expressions](https://github.com/LightBDD/LightBDD/wiki/Expectation-Expressions),
* [Step comments](https://github.com/LightBDD/LightBDD/wiki/Step-Comments),
* [Scenario and Step Decorators](https://github.com/LightBDD/LightBDD/wiki/Extending-Test-Behavior).

### Example console output during tests execution:
What LightBDD offers for free is the scenario output provided in Visual Studio/Resharper test screens and/or Console window.  
Where possible, the output is provided as the test executes, allowing to track the progress of the scenarios.

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

For more information, please visit [Test Progress Notification](https://github.com/LightBDD/LightBDD/wiki/Test-Progress-Notification) page.

### Example HTML report generated after test execution:
When all tests are finished, LightBDD generates the HTML report file, providing the summary and the details of the executed scenarios - the fragment of the report is presented below.

![](https://raw.githubusercontent.com/wiki/LightBDD/LightBDD/images/scenario_report.png)

To read more, please see [Generating Reports](https://github.com/LightBDD/LightBDD/wiki/Generating-Reports) wiki page.

## Installation
The LightBDD is available on [NuGet](https://www.nuget.org/packages?q=LightBDD): 

* `PM> Install-Package LightBDD.NUnit3` for package using NUnit 3x
* `PM> Install-Package LightBDD.XUnit2` for package using xUnit 2x
* `PM> Install-Package LightBDD.MsTest2` for package using MsTest.TestFramework
* `PM> Install-Package LightBDD.Fixie2` for package using Fixie 2x

## Productivity Extensions for Visual Studio
**LightBDD** project offers also a VSIX extension for Visual Studio, containing:
* [Project Templates](https://github.com/LightBDD/LightBDD/wiki/Visual-Studio-Extensions#project-templates) for all integrations (nunit/xunit/mstest/fixie),
* [Feature Class Templates](https://github.com/LightBDD/LightBDD/wiki/Visual-Studio-Extensions#item-templates) for all integrations (nunit/xunit/mstest/fixie),
* [Code Snippets](https://github.com/LightBDD/LightBDD/wiki/Visual-Studio-Extensions#code-snippets) for creating Scenarios and Composite Steps.

The VSIX extension can be downloaded from: [Visual Studio Gallery](https://marketplace.visualstudio.com/items?itemName=Suremaker.lightbdd) (Visual Studio 2012 and newer versions supported).  

## Migrating LightBDD
The current LightBDD version series is **3.X**. To find out how to migrate from previous versions, please visit [Migrating LightBDD Versions](https://github.com/LightBDD/LightBDD/wiki/Migrating-LightBDD-Versions) wiki page.

## Debugging LightBDD from NuGet packages
LightBDD provides debug symbols helping with diagnostics - please check [Debugging LightBDD Scenarios](https://github.com/LightBDD/LightBDD/wiki/Debugging-LightBDD-Scenarios) for details.

## More information about LightBDD
If you are interested about background of LightBDD creation or getting insight into what is driving it's evolution, please feel free to take a look at my [blog posts](https://woitech.eu/blog/category/lightbdd/).

## Community Projects
* [LightBdd.TabularAttributes](https://github.com/lemonlion/LightBdd.TabularAttributes) - Designed to be used with LightBdd Tabular Parameters to allow for specifying input and output data for tests via attributes, in a truth table format.
