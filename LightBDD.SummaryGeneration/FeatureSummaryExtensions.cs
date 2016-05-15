using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution.Results;

namespace LightBDD.SummaryGeneration
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
            return results.SelectMany(r => r.GetScenarios())
            .Where(s => s.ExecutionTime != null)
            .Select(s => (DateTimeOffset?)s.ExecutionTime.Start)
            .OrderBy(s => s)
            .FirstOrDefault()
            .GetValueOrDefault(DateTimeOffset.UtcNow);
        }

        /// <summary>
        /// Returns total test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestExecutionTime(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(r => r.GetScenarios()).GetTestExecutionTime();
        }

        /// <summary>
        /// Returns total test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestExecutionTime(this IEnumerable<IScenarioResult> results)
        {
            return results.Aggregate(TimeSpan.Zero, (current, s) => current + (s.ExecutionTime?.Duration).GetValueOrDefault());
        }

        /// <summary>
        /// Returns average test execution time based on Scenario ExecutionTime property.
        /// </summary>
        public static TimeSpan GetTestAverageExecutionTime(this IEnumerable<IScenarioResult> results)
        {
            return TimeSpan.FromTicks((long)results.Select(s => (s.ExecutionTime?.Duration).GetValueOrDefault().Ticks).DefaultIfEmpty(0).Average());
        }

        /// <summary>
        /// Counts scenarios that belongs to given feature and have given status.
        /// </summary>
        public static int CountScenariosWithStatus(this IFeatureResult result, ExecutionStatus status)
        {
            return result.GetScenarios().Count(s => s.Status == status);
        }

        /// <summary>
        /// Counts all scenarios with given status.
        /// </summary>
        public static int CountScenariosWithStatus(this IEnumerable<IFeatureResult> results, ExecutionStatus status)
        {
            return results.Sum(f => f.CountScenariosWithStatus(status));
        }

        /// <summary>
        /// Counts all scenarios.
        /// </summary>
        public static int CountScenarios(this IEnumerable<IFeatureResult> results)
        {
            return results.SelectMany(f => f.GetScenarios()).Count();
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
            return result.GetScenarios().SelectMany(s => s.GetSteps()).Count();
        }

        /// <summary>
        /// Counts all steps with given status.
        /// </summary>
        public static int CountStepsWithStatus(this IFeatureResult result, ExecutionStatus status)
        {
            return result.GetScenarios().Sum(s => s.CountStepsWithStatus(status));
        }

        /// <summary>
        /// Counts all steps with given status.
        /// </summary>
        public static int CountStepsWithStatus(this IScenarioResult result, ExecutionStatus status)
        {
            return result.GetSteps().Count(s => s.Status == status);
        }

        /// <summary>
        /// Counts all steps with given status.
        /// </summary>
        public static int CountStepsWithStatus(this IEnumerable<IFeatureResult> results, ExecutionStatus status)
        {
            return results.Sum(f => f.CountStepsWithStatus(status));
        }
    }
}