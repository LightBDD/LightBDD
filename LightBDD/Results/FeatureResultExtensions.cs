using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Results
{
    /// <summary>
    /// Helper methods to work on feature results
    /// </summary>
    internal static class FeatureResultExtensions
    {
        /// <summary>
        /// Returns time when first scenario has started executing, basing on Scenario ExecutionStart property.
        /// If no scenarios have been executed, null is returned.
        /// </summary>
        public static DateTimeOffset? GetTestExecutionStartTime(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(r => r.Scenarios)
            .Where(s => s.ExecutionStart != null)
            .Select(s => s.ExecutionStart)
            .OrderBy(s => s.GetValueOrDefault())
            .FirstOrDefault();
        }

        /// <summary>
        /// Return total test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestExecutionTime(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(r => r.Scenarios).GetTestExecutionTime();
        }

        /// <summary>
        /// Return total test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestExecutionTime(this IEnumerable<IScenarioResult> results)
        {
            return results.Aggregate(TimeSpan.Zero, (current, s) => current + s.ExecutionTime.GetValueOrDefault());
        }
    }
}