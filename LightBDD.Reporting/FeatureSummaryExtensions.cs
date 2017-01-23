using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;

namespace LightBDD.Reporting
{
    /// <summary>
    /// Helper methods to work on feature results
    /// </summary>
    public static class FeatureSummaryExtensions
    {
        public static ExecutionTimeSummary GetTestExecutionTimeSummary(this IEnumerable<IFeatureResult> results)
        {
            return ExecutionTimeSummary.Calculate(
                results
                .SelectMany(r => r.GetScenarios())
                .Select(s => s.ExecutionTime)
                .Where(s => s != null));
        }

        public static ExecutionTimeSummary GetTestExecutionTimeSummary(this IEnumerable<IScenarioResult> results)
        {
            return ExecutionTimeSummary.Calculate(
                results
                .Select(s => s.ExecutionTime)
                .Where(s => s != null));
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
            return results.Sum(f => CountScenariosWithStatus(f, status));
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
            return results.Sum(f => CountSteps(f));
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