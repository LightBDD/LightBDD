using System;
using System.Reflection;
using LightBDD.Integration;
using Xunit.Sdk;

namespace LightBDD
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// It is required for running LightBDD tests with XUnit testing framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : BeforeAfterTestAttribute
    {
        /// <summary>
        /// This method is called after the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void After(MethodInfo methodUnderTest)
        {
            TestMethodInfoProvider.TestMethod = null;
        }

        /// <summary>
        /// This method is called before the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void Before(MethodInfo methodUnderTest)
        {
            TestMethodInfoProvider.TestMethod = methodUnderTest;
        }
    }
}