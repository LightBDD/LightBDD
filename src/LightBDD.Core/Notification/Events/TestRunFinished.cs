using LightBDD.Core.Execution;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification.Events;

/// <summary>
/// Event raised when test run is finished.
/// </summary>
public class TestRunFinished : ProgressEvent
{
    /// <summary>
    /// Test run result.
    /// </summary>
    public ITestRunResult Result { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public TestRunFinished(EventTime time, ITestRunResult result) : base(time)
    {
        Result = result;
    }
}