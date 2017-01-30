using System;
using LightBDD.Integration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace LightBDD
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// It is required for running LightBDD tests with NUnit 3 testing framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : Attribute, ITestAction
    {
        /// <summary>Executed before each test is run</summary>
        /// <param name="test">The test that is going to be run.</param>
        public void BeforeTest(ITest test)
        {
            // TODO: TestContext.Out assignment to TestContextProvider is really a workaround for https://github.com/nunit/nunit/issues/1965 and should be removed when fixed
            TestContextProvider.Initialize(test.Method.MethodInfo, TestContext.Out);
        }

        /// <summary>Executed after each test is run</summary>
        /// <param name="test">The test that has just been run.</param>
        public void AfterTest(ITest test)
        {
            TestContextProvider.Clear();
        }

        /// <summary>Provides the target for the action attribute</summary>
        /// <returns>The target for the action attribute</returns>
        public ActionTargets Targets => ActionTargets.Test;
    }
}