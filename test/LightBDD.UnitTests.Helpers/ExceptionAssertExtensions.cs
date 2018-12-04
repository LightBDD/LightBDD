using System;
using NUnit.Framework;

namespace LightBDD.UnitTests.Helpers
{
    public static class ExceptionAssertExtensions
    {
        public static void AssertStackTraceMatching(this Exception ex, string expectedStackTracePattern)
        {
            var actual = ex.StackTrace.Replace("\r", "").Replace("--- End of stack trace from previous location where exception was thrown ---\n","");
            Assert.That(actual, Does.Match(expectedStackTracePattern.Replace("\r", "")));
        }
    }
}