#nullable enable
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification.Implementation
{
    class DelegatingProgressNotifier : IProgressNotifier
    {
        private readonly IList<IProgressNotifier> _notifiers;
        private DelegatingProgressNotifier(IList<IProgressNotifier> notifiers)
        {
            _notifiers = notifiers;
        }

        public void Notify(ProgressEvent e)
        {
            foreach (var notifier in _notifiers)
                notifier.Notify(e);
        }

        public static IProgressNotifier Compose(IEnumerable<IProgressNotifier> notifiers)
        {
            var result = Flatten(notifiers, new List<IProgressNotifier>());
            if (result.Count > 1)
                return new DelegatingProgressNotifier(result);

            return result.FirstOrDefault() ?? NoProgressNotifier.Default;
        }

        private static List<IProgressNotifier> Flatten(IEnumerable<IProgressNotifier> notifiers, List<IProgressNotifier> results)
        {
            foreach (var notifier in notifiers ?? Enumerable.Empty<IProgressNotifier>())
            {
                if (notifier == null || notifier is NoProgressNotifier)
                    continue;
                if (notifier is DelegatingProgressNotifier dp)
                    Flatten(dp._notifiers, results);
                else
                    results.Add(notifier);
            }

            return results;
        }
    }
}
