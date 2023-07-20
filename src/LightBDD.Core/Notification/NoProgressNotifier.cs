using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Progress notifier implementation that does nothing when called.
    /// </summary>
    public class NoProgressNotifier : IProgressNotifier
    {
        private NoProgressNotifier() { }

        /// <summary>
        /// Returns default instance.
        /// </summary>
        public static NoProgressNotifier Default { get; } = new();

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Notify(ProgressEvent e) { }
    }
}