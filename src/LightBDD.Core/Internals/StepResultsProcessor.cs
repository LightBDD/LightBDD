using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Results;

namespace LightBDD.Core.Internals
{
    internal static class StepResultsProcessor
    {
        public static string MergeStatusDetails(this IEnumerable<IStepResult> subSteps, string currentDetails = null)
        {
            var details = string.IsNullOrWhiteSpace(currentDetails)
                ? Enumerable.Empty<string>()
                : Enumerable.Repeat(currentDetails, 1);
            details = details.Concat(subSteps.Where(s => s.StatusDetails != null).Select(s => s.StatusDetails));
            var merged = string.Join(Environment.NewLine, details);
            return merged.Length > 0 ? merged : null;
        }

        public static IStepResult GetMostSevereOrNull(this IEnumerable<IStepResult> results)
        {
            return results.Reverse().OrderByDescending(s => s.Status).FirstOrDefault();
        }
    }
}