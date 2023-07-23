using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;

namespace LightBDD.Core.UnitTests.Helpers;

public class CapturingProgressNotifier : IProgressNotifier
{
    private readonly ConcurrentQueue<ProgressEvent> _events = new();
    public IEnumerable<ProgressEvent> Events => _events;
    public void Notify(ProgressEvent e) => _events.Enqueue(e);
}