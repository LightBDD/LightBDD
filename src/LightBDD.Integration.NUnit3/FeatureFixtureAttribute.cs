using System;
using LightBDD.Integration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace LightBDD
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestMethodInfoProvider.TestMethod = test.Method.MethodInfo;
        }

        public void AfterTest(ITest test)
        {
            TestMethodInfoProvider.TestMethod = null;
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}