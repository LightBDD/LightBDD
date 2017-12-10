using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Internals;
using LightBDD.Framework.Notification.Implementation.IncrementalJson;
using LightBDD.Framework.Reporting.Implementation;

namespace LightBDD.Framework.Notification
{
    public class IncrementalJsonProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier, ILightBddProgressNotifier
    {
        private readonly AsyncConcurrentQueue<JsonElement> _queue = new AsyncConcurrentQueue<JsonElement>();
        private readonly Stopwatch _timeLine = new Stopwatch();
        private readonly Task _backgroundWriter;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly FileStream _fileStream;

        public IncrementalJsonProgressNotifier(string outputPath)
        {
            var filePath = FilePathHelper.ResolveAbsolutePath(outputPath);
            FilePathHelper.EnsureOutputDirectoryExists(filePath);
            _fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            _backgroundWriter = Task.Run(SaveMessagesAsync);
        }

        private async Task SaveMessagesAsync()
        {
            using (_fileStream)
            using (var writer = new InlineJsonStreamWriter(_fileStream))
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var message = await _queue.DequeueAsync(_cancellationTokenSource.Token);
                        WriteMessage(message, writer);
                    }
                    catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested) { }
                }

                foreach (var message in _queue)
                    WriteMessage(message, writer);
            }
        }

        private static void WriteMessage(JsonElement message, InlineJsonStreamWriter writer)
        {
            message.WriteTo(writer);
            writer.WriteDirect('\n');
            writer.Flush();
        }

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