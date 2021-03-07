using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.IO;
using LightBDD.Reporting.Progressive.Mappers;
using FeatureFinished = LightBDD.Core.Notification.Events.FeatureFinished;
using FeatureStarting = LightBDD.Core.Notification.Events.FeatureStarting;
using ScenarioFinished = LightBDD.Core.Notification.Events.ScenarioFinished;
using ScenarioStarting = LightBDD.Core.Notification.Events.ScenarioStarting;
using StepCommented = LightBDD.Core.Notification.Events.StepCommented;
using StepFinished = LightBDD.Core.Notification.Events.StepFinished;
using StepStarting = LightBDD.Core.Notification.Events.StepStarting;

namespace LightBDD.Reporting.Progressive
{
    internal class JsonlProgressNotifier : IProgressNotifier, IDisposable
    {
        private readonly Func<string, Task> _onWrite;
        private readonly ConcurrentQueue<ProgressEvent> _queue = new ConcurrentQueue<ProgressEvent>();
        private readonly SemaphoreSlim _sem = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private JsonlEventSerializer _serializer=new JsonlEventSerializer();
        private readonly Task _writingTask;

        public JsonlProgressNotifier(Func<string,Task> onWrite)
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

        private async Task ProcessEvent(ProgressEvent e)
        {
            switch (e)
            {
                case FeatureFinished featureFinished:
                    await Handle(featureFinished);
                    break;
                case FeatureStarting featureStarting:
                    await Handle(featureStarting);
                    break;
                case ScenarioFinished scenarioFinished:
                    await Handle(scenarioFinished);
                    break;
                case ScenarioStarting scenarioStarting:
                    await Handle(scenarioStarting);
                    break;
                case StepCommented stepCommented:
                    await Handle(stepCommented);
                    break;
                case StepFinished stepFinished:
                    await Handle(stepFinished);
                    break;
                case StepStarting stepStarting:
                    await Handle(stepStarting);
                    break;
            }
        }

        private async Task Handle(StepStarting e)
        {
            await Write(EventMapper.ToStepDiscovered(e));
            await Write(EventMapper.ToStepStarting(e));
        }

        private async Task Write(Event e) => await _onWrite(_serializer.Serialize(e));

        private async Task Handle(StepFinished e)
        {
            await Write(EventMapper.ToStepFinished(e));
        }

        private async Task Handle(StepCommented e)
        {
            await Write(EventMapper.ToStepCommented(e));
        }

        private async Task Handle(ScenarioFinished e)
        {
            await Write(EventMapper.ToScenarioFinished(e));
        }

        private async Task Handle(ScenarioStarting e)
        {
            await Write(EventMapper.ToScenarioDiscovered(e));
            await Write(EventMapper.ToScenarioStarting(e));
        }

        private async Task Handle(FeatureFinished e)
        {
            await Write(EventMapper.ToFeatureFinished(e));
        }

        private async Task Handle(FeatureStarting e)
        {
            await Write(e.ToFeatureDiscovered());
            await Write(e.ToFeatureStarting());
        }

        public void Notify(ProgressEvent e)
        {
            _queue.Enqueue(e);
            _sem.Release();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _writingTask.Wait();
        }
    }
}
