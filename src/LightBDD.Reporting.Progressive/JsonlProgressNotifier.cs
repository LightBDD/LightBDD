using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.IO;
using LightBDD.Reporting.Progressive.Mappers;

namespace LightBDD.Reporting.Progressive
{
    /// <summary>
    /// Notifier that serializes the notification in the series of json lines in thread safe manner.
    /// </summary>
    public class JsonlProgressNotifier : IProgressNotifier, IDisposable
    {
        private readonly Func<string, Task> _onWrite;
        private readonly ConcurrentQueue<NotificationEvent> _queue = new ConcurrentQueue<NotificationEvent>();
        private readonly SemaphoreSlim _sem = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private static readonly JsonlEventSerializer _serializer = new JsonlEventSerializer();
        private readonly Task _writingTask;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonlProgressNotifier(Func<string, Task> onWrite)
        {
            _onWrite = onWrite;
            _writingTask = Task.Run(WriteLoop);
        }

        private async Task WriteLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    await _sem.WaitAsync(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (_queue.TryDequeue(out var e))
                    await ProcessEvent(e);
            }

            while (_queue.TryDequeue(out var e))
                await ProcessEvent(e);
        }

        private async Task ProcessEvent(NotificationEvent e)
        {
            await _onWrite(_serializer.Serialize(e));
        }

        /// <inheritdoc />
        public void Notify(ProgressEvent e)
        {
            foreach (var mappedEvent in EventMapper.Map(e))
            {
                _queue.Enqueue(mappedEvent);
                _sem.Release();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _cts.Cancel();
            _writingTask.Wait();
        }
    }
}
