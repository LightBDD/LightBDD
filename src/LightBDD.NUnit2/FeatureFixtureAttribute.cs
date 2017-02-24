using System;
using LightBDD.NUnit2.Implementation;
using NUnit.Framework;

namespace LightBDD.NUnit2
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// It is required for running LightBDD tests with NUnit testing framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : Attribute, ITestAction
    {
        /// <summary>Executed before each test is run</summary>
        /// <param name="testDetails">Provides details about the test that is going to be run.</param>
        public void BeforeTest(TestDetails testDetails)
        {
            TestContextProvider.Initialize(testDetails.Method);
        }

        /// <summary>Executed after each test is run</summary>
        /// <param name="testDetails">Provides details about the test that has just been run.</param>
        public void AfterTest(TestDetails testDetails)
        {
            TestContextProvider.Clear();
        }

        /// <summary>Provides the target for the action attribute</summary>
        /// <returns>The target for the action attribute</returns>
        public ActionTargets Targets => ActionTargets.Test;
    }
}