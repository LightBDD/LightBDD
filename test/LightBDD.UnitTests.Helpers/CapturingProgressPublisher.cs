using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;

namespace LightBDD.UnitTests.Helpers
{
    public class CapturingProgressPublisher : IProgressPublisher
    {
        private readonly Stopwatch _watch = Stopwatch.StartNew();
        private readonly ConcurrentQueue<ProgressEvent> _events = new ConcurrentQueue<ProgressEvent>();

        public IEnumerable<TEvent> GetCaptured<TEvent>() where TEvent : ProgressEvent => _events.OfType<TEvent>();
        public IEnumerable<ProgressEvent> GetCaptured() => _events;

        public void Publish<TEvent>(Func<EventTime, TEvent> eventFn) where TEvent : ProgressEvent
        {
            _events.Enqueue(eventFn(new EventTime(DateTimeOffset.MinValue, _watch.Elapsed)));
        }
    }
}