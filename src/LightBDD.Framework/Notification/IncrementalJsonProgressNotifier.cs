using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Internals;
using LightBDD.Framework.Notification.Implementation.IncrementalJson;

namespace LightBDD.Framework.Notification
{
    public abstract class IncrementalJsonProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier, ILightBddProgressNotifier
    {
        private readonly AsyncConcurrentQueue<JsonElement> _queue = new AsyncConcurrentQueue<JsonElement>();
        private readonly Stopwatch _timeLine = new Stopwatch();
        private Task _backgroundWriter;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private async Task SaveMessagesAsync()
        {
            try
            {
                OnStart();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var message = await _queue.DequeueAsync(_cancellationTokenSource.Token);
                        WriteMessage(message.WriteTo);
                    }
                    catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
                    {
                    }
                }

                foreach (var message in _queue)
                    WriteMessage(message.WriteTo);
            }
            finally
            {
                OnFinish();
            }
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnFinish()
        {
        }

        protected abstract void WriteMessage(Action<StreamWriter> writeFn);

        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _queue.Enqueue(scenario.ToJson(_timeLine));
        }

        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            _queue.Enqueue(scenario.ToJson(_timeLine));
        }

        public void NotifyStepStart(IStepInfo step)
        {
            _queue.Enqueue(step.ToJson(_timeLine));
        }

        public void NotifyStepFinished(IStepResult step)
        {
            _queue.Enqueue(step.ToJson(_timeLine));
        }

        public void NotifyStepComment(IStepInfo step, string comment)
        {
            _queue.Enqueue(step.ToStepCommentJson(_timeLine, comment));
        }

        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            _queue.Enqueue(feature.ToJson(_timeLine));
        }

        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            _queue.Enqueue(feature.ToJson(_timeLine));
        }

        public void NotifyLightBddStart()
        {
            _backgroundWriter = Task.Run(SaveMessagesAsync);
            _timeLine.Start();
        }

        public void NotifyLightBddFinished()
        {
            _queue.Enqueue(JsonFormatter.ToTestsFinished(_timeLine));
            _cancellationTokenSource.Cancel();
            _backgroundWriter.GetAwaiter().GetResult();
        }
    }
}