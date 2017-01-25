using System;
using System.Reflection;
using LightBDD.Integration;
using Xunit.Sdk;

namespace LightBDD
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : BeforeAfterTestAttribute
    {
        public override void After(MethodInfo methodUnderTest)
        {
            TestMethodInfoProvider.TestMethod = null;
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            TestMethodInfoProvider.TestMethod = methodUnderTest;
        }
    }
}