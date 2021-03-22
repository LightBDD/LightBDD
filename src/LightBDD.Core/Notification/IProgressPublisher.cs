using System;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.Notification
{
    /// <summary>
    /// Progress publishing interface for publishing time-series progress events.
    /// </summary>
    public interface IProgressPublisher
    {
        /// <summary>
        /// Publish progress event created by <paramref name="eventFn"/>.
        /// </summary>
        void Publish<TEvent>(Func<EventTime, TEvent> eventFn) where TEvent : ProgressEvent;
    }
}