using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Results
{
    /// <summary>
    /// Helper methods to work on feature results
    /// </summary>
    public static class FeatureSummaryExtensions
    {
        /// <summary>
        /// Returns time when first scenario has started executing based on Scenario ExecutionStart property.
        /// If no scenarios have been executed, UtcNow is returned.
        /// </summary>
        public static DateTimeOffset GetTestExecutionStartTime(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(r => r.Scenarios)
            .Where(s => s.ExecutionStart.HasValue)
            .Select(s => s.ExecutionStart)
            .OrderBy(s => s)
            .FirstOrDefault()
            .GetValueOrDefault(DateTimeOffset.UtcNow);
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
        public static int CountScenariosWithStatus(this IFeatureResult result, ResultStatus resultStatus)
        {
            return result.Scenarios.Count(s => s.Status == resultStatus);
        }

        /// <summary>
        /// Counts all scenarios with given status.
        /// </summary>
        public static int CountScenariosWithStatus(this IEnumerable<IFeatureResult> results, ResultStatus status)
        {
            return results.Sum(f => f.CountScenariosWithStatus(status));
        }

        /// <summary>
        /// Counts all scenarios.
        /// </summary>
        public static int CountScenarios(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(f => f.Scenarios).Count();
        }

        /// <summary>
        /// Counts all steps.
        /// </summary>
        public static int CountSteps(this IEnumerable<IFeatureResult> results)
        {
            return results.Sum(f => f.CountSteps());
        }

        /// <summary>
        /// Counts all steps.
        /// </summary>
        public static int CountSteps(this IFeatureResult result)
        {
            return result.Scenarios.SelectMany(s => s.Steps).Count();
        }

        /// <summary>
        /// Counts all steps with given status.
        /// </summary>
        public static int CountStepsWithStatus(this IFeatureResult result, ResultStatus resultStatus)
        {
            return result.Scenarios.Sum(s => s.CountStepsWithStatus(resultStatus));
        }

        /// <summary>
        /// Counts all steps with given status.
        /// </summary>
        public static int CountStepsWithStatus(this IScenarioResult result, ResultStatus resultStatus)
        {
            return result.Steps.Count(s => s.Status == resultStatus);
        }

        /// <summary>
        /// Counts all steps with given status.
        /// </summary>
        public static int CountStepsWithStatus(this IEnumerable<IFeatureResult> results, ResultStatus status)
        {
            return results.Sum(f => f.CountStepsWithStatus(status));
        }
    }
}