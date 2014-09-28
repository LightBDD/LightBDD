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
        /// Returns time when first scenario has started executing based on Scenario ExecutionStart property.
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
        /// Returns total test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestExecutionTime(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(r => r.Scenarios).GetTestExecutionTime();
        }

        /// <summary>
        /// Returns total test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestExecutionTime(this IEnumerable<IScenarioResult> results)
        {
            return results.Aggregate(TimeSpan.Zero, (current, s) => current + s.ExecutionTime.GetValueOrDefault());
        }

        /// <summary>
        /// Counts scenarios that belongs to given feature and have given status.
        /// </summary>
        public static int CountScenarios(this IFeatureResult feature, ResultStatus resultStatus)
        {
            return feature.Scenarios.Count(s => s.Status == resultStatus);
        }
    }
}