using System;
using System.Diagnostics;

namespace LightBDD.Notification
{
    /// <summary>
    /// Progress notifier using console for displaying progress.
    /// </summary>
    [DebuggerStepThrough]
    public class ConsoleProgressNotifier : TextProgressNotifier
    {
        /// <summary>
        /// Uses Console to write preformatted notifications
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="args">Args.</param>
        protected override void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}