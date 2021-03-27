using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Notification.Jsonl.IO;
using LightBDD.Reporting.Progressive.Mappers;

namespace LightBDD.Reporting.Progressive
{
    /// <summary>
    /// Jsonl progress notifier.
    /// </summary>
    public class JsonlProgressNotifier : IProgressNotifier, IDisposable
    {
        private readonly ConcurrentQueue<ProgressEvent> _queue = new ConcurrentQueue<ProgressEvent>();
        private readonly SemaphoreSlim _sem = new SemaphoreSlim(0);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private JsonlEventWriter _writer;
        private readonly Task _writingTask;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonlProgressNotifier(Stream outputUtf8Stream)
        {
            _writer = new JsonlEventWriter(outputUtf8Stream);
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
            await _writer.Write(EventMapper.ToStepDiscovered(e));
            await _writer.Write(EventMapper.ToStepStarting(e));
        }

        private async Task Handle(StepFinished e)
        {
            await _writer.Write(EventMapper.ToStepFinished(e));
        }

        private async Task Handle(StepCommented e)
        {
            await _writer.Write(EventMapper.ToStepCommented(e));
        }

        private async Task Handle(ScenarioFinished e)
        {
            await _writer.Write(EventMapper.ToScenarioFinished(e));
        }

        private async Task Handle(ScenarioStarting e)
        {
            await _writer.Write(EventMapper.ToScenarioDiscovered(e));
            await _writer.Write(EventMapper.ToScenarioStarting(e));
        }

        private async Task Handle(FeatureFinished e)
        {
            await _writer.Write(EventMapper.ToFeatureFinished(e));
        }

        private async Task Handle(FeatureStarting e)
        {
            await _writer.Write(e.ToFeatureDiscovered());
            await _writer.Write(e.ToFeatureStarting());
        }

        /// <inheritdoc />
        public void Notify(ProgressEvent e)
        {
            _queue.Enqueue(e);
            _sem.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _cts.Cancel();
            _writingTask.Wait();
        }
    }
}
