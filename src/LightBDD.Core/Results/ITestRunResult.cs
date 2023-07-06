using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results;

/// <summary>
/// Interface describing test run.
/// </summary>
public interface ITestRunResult
{
    /// <summary>
    /// Test run details.
    /// </summary>
    ITestRunInfo Info { get; }
    /// <summary>
    /// Overall status that can be either <seealso cref="ExecutionStatus.Passed"/> or <seealso cref="ExecutionStatus.Failed"/>
    /// </summary>
    ExecutionStatus OverallStatus { get; }
    /// <summary>
    /// Overall execution time
    /// </summary>
    ExecutionTime ExecutionTime { get; }
    /// <summary>
    /// Returns results for features covered by this test run, where the features are ordered by Name.
    /// </summary>
    IEnumerable<IFeatureResult> GetFeatures();

    //setup and teardown results
}