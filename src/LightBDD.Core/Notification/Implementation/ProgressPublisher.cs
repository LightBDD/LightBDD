using System;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification.Implementation
{
    internal class ProgressPublisher : IProgressPublisher
    {
        private readonly IProgressNotifier _notifier;
        private readonly IExecutionTimer _timer;

        public ProgressPublisher(IProgressNotifier notifier, IExecutionTimer timer)
        {
            _notifier = notifier;
            _timer = timer;
        }

        public void Publish<TEvent>(Func<EventTime, TEvent> eventFn) where TEvent : ProgressEvent
        {
            _notifier.Notify(eventFn(_timer.GetTime()));
        }
    }
}