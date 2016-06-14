using System.Diagnostics;
using NUnit.Framework;

namespace LightBDD.Notification
{
    /// <summary>
    /// Progress notifier using test output helper for displaying progress.
    /// </summary>
    [DebuggerStepThrough]
    public class NUnit3OutputProgressNotifier : TextProgressNotifier
    {
        /// <summary>
        /// Constructor initializing notifier with test out helper.
        /// </summary>
        public NUnit3OutputProgressNotifier()
        {
        }

        /// <summary>
        /// Writes notifications using output helper.
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="args">Args.</param>
        protected override void WriteLine(string format, params object[] args)
        {
            TestContext.Out.WriteLine(format, args);
        }
    }
}
