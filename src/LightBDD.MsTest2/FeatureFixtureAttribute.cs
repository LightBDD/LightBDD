using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2
{
    /// <summary>
    /// Attribute that should be applied to all test classes representing feature tests.
    /// It should be used for running LightBDD tests with MSTest testing framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : TestClassAttribute
    {
    }
}