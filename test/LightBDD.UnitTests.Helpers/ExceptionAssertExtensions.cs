using System;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public static class ExceptionAssertExtensions
    {
        public static void AssertStackTraceMatching(this Exception ex, string expectedStackTracePattern)
        {
            Assert.That(ex.StackTrace.Replace("\r", ""), Does.Match(expectedStackTracePattern.Replace("\r", "")));
        }
    }
}