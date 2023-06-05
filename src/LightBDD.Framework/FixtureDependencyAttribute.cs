using System;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework
{
    /// <summary>
    /// Attribute marking fixture class public properties to have values injected from DI before each scenario execution.<br/><br/>
    /// The property values are injected during scenario execution by one of <c>RunScenario()</c>/<c>RunScenarioAsync()</c>/<c>RunAsync()</c> runner method family and they are available within the scope of: <br/>
    /// - step methods,<br/>
    /// - <seealso cref="IScenarioSetUp.OnScenarioSetUp"/> and <seealso cref="IScenarioTearDown.OnScenarioTearDown"/> methods,<br/>
    /// - <seealso cref="IScenarioDecorator"/> and <seealso cref="IStepDecorator"/> decorator methods,<br/>
    /// - scenario step parameters provided during scenario specification using <seealso cref="ExtendedExtensions"/> <c>RunScenario()</c>/<c>RunScenarioAsync()</c>/<c>AddSteps()</c>/<c>AddAsyncSteps()</c> methods like:
    /// <code>
    ///     [FeatureDependency]
    ///     public TestUsers Users {get; set;}
    /// 
    ///     [Scenario]
    ///     public void Some_scenario()
    ///     {
    ///         Runner.RunScenario(
    ///             _ => Given_I_am_logged_in_as_user(Users.Admin),
    ///             /* ... */
    ///         );
    ///     }
    /// </code><br/><br/>
    /// Please note that fixture class properties are not injected yet when accessed from fixture constructors, underlying test framework setup methods (such as NUnit <c>[SetUp]</c> methods),
    /// scenario methods before calling <c>RunScenario()</c>/<c>RunScenarioAsync()</c>/<c>RunAsync()</c> due to LightBDD integration limitations.<br/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FixtureDependencyAttribute : Attribute, IFixtureDependencyAttribute { }
}