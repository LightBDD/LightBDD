#nullable enable
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Progress notification dispatcher used to dispatch notifications.
    /// </summary>
    //TODO: make internal?
    public class ProgressNotificationDispatcher
    {
        private readonly IReadOnlyList<IProgressNotifier> _notifiers;
        /// <summary>
        /// Default constructor
        /// </summary>
        public ProgressNotificationDispatcher(IEnumerable<IProgressNotifier> notifiers)
        {
            _notifiers = notifiers.ToArray();
        }

        /// <summary>
        /// Dispatches <see cref="ProgressEvent"/> notification.
        /// </summary>
        /// <param name="e">Event to dispatch</param>
        public void Notify(ProgressEvent e)
        {
            foreach (var notifier in _notifiers)
                notifier.Notify(e);
        }
    }
}
