using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Progress notification interface used to send <see cref="ProgressEvent"/> notifications during test execution.
    /// </summary>
    //TODO: the notification and reporting needs to be separated from events, make events core concept with notifiers allowing user notifications (like console progress output) while reporters allowing to generate reports, both based on core events.
    public interface IProgressNotifier
    {
        /// <summary>
        /// The method is called during test execution to notify about specific <see cref="ProgressEvent"/> occurrence.
        /// </summary>
        /// <param name="e">Event</param>
        void Notify(ProgressEvent e);
    }
}