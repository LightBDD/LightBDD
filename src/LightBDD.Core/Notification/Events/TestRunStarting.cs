using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events;

/// <summary>
/// Event raised when test run is about to start.
/// </summary>
public class TestRunStarting : ProgressEvent
{
    /// <summary>
    /// Test Run
    /// </summary>
    public ITestRunInfo TestRun { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public TestRunStarting(EventTime time, ITestRunInfo testRun) : base(time)
    {
        TestRun = testRun;
    }
}