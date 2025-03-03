using System;
using System.Threading.Tasks;
using LightBDD.TUnit.Implementation;
using TUnit.Core.Interfaces;

namespace LightBDD.TUnit
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// It is required for running LightBDD tests with NUnit testing framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : Attribute, ITestStartEventReceiver, ITestEndEventReceiver
    {
        /// <summary>Executed before each test is run</summary>
        /// <param name="beforeTestContext">The test that is going to be run.</param>
        public ValueTask OnTestStart(BeforeTestContext beforeTestContext)
        {
            TestContextProvider.Initialize(beforeTestContext.TestDetails.TestMethod.ReflectionInformation, beforeTestContext.TestDetails.TestMethodArguments);
            return default;
        }

        /// <summary>Executed after each test is run</summary>
        /// <param name="testContext">The test that has just been run.</param>
        public ValueTask OnTestEnd(TestContext testContext)
        {
            TestContextProvider.Clear();
            return default;
        }

        /// <summary>
        /// The order of the test event receiver.
        /// </summary>
        public int Order { get; }
    }
}