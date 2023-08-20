using System;
using Shouldly;

namespace LightBDD.ScenarioHelpers
{
    public static class ExceptionAssertExtensions
    {
        public static void AssertStackTraceMatching(this Exception ex, string expectedStackTracePattern)
        {
            var actual = ex.StackTrace?.NormalizeNewLine().Replace("--- End of stack trace from previous location where exception was thrown ---\n", "") ?? string.Empty;
            var expected = expectedStackTracePattern.NormalizeNewLine();
            actual.ShouldMatch(expected);
        }
    }
}