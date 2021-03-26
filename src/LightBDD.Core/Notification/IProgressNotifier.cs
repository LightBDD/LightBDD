using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Progress notification interface used to send and handle <see cref="ProgressEvent"/> notifications during test execution.
    /// </summary>
    //TODO: consider renaming to notification handler
    public interface IProgressNotifier
    {
        /// <summary>
        /// The method is called during test execution to notify about specific <see cref="ProgressEvent"/> occurrence.
        /// </summary>
        /// <param name="e">Event</param>
        void Notify(ProgressEvent e);
    }
}