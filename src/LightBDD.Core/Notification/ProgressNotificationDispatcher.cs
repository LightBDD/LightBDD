#nullable enable
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification
{
    public class ProgressNotificationDispatcher
    {
        private readonly IReadOnlyList<IProgressNotifier> _notifiers;
        public ProgressNotificationDispatcher(IEnumerable<IProgressNotifier> notifiers)
        {
            _notifiers = notifiers.ToArray();
        }

        public void Notify(ProgressEvent e)
        {
            foreach (var notifier in _notifiers)
                notifier.Notify(e);
        }
    }
}
