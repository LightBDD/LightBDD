using System.Collections.Generic;
using System.Reflection;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// Interface describing test execution result.
    /// </summary>
    public interface ITestExecutionResult
    {
        /// <summary>
        /// Returns execution time for all features.
        /// </summary>
        ExecutionTime ExecutionTime { get; }
        /// <summary>
        /// Returns results of features executed within this test run.
        /// </summary>
        IEnumerable<IFeatureResult> GetFeatures();
    }
}
