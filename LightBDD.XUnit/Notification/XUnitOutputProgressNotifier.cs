using System.Diagnostics;
using Xunit.Abstractions;

namespace LightBDD.Notification
{
    /// <summary>
    /// Progress notifier using test output helper for displaying progress.
    /// </summary>
    [DebuggerStepThrough]
    public class XUnitOutputProgressNotifier : TextProgressNotifier
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Constructor initializing notifier with test out helper.
        /// </summary>
        public XUnitOutputProgressNotifier(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// Writes notifications using output helper.
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="args">Args.</param>
        protected override void WriteLine(string format, params object[] args)
        {
            _output.WriteLine(format, args);
        }
    }
}
