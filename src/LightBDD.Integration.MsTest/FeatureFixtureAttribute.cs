using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureFixtureAttribute : TestClassAttribute
    {
    }
}