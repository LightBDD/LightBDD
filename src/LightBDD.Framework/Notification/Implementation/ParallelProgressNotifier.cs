using System;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification.Implementation
{
    internal class ParallelProgressNotifier : IProgressNotifier
    {
        private readonly ProgressManager _manager;
        private readonly DefaultProgressNotifier _notifier;
        private readonly Action<string> _onNotify;

        public ParallelProgressNotifier(ProgressManager manager, Action<string>[] onNotify)
        {
            if (onNotify == null || !onNotify.Any())
                throw new ArgumentException("At least one on notify action required", nameof(onNotify));

            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _onNotify = onNotify.Aggregate((current, next) => current + next);
            _notifier = new DefaultProgressNotifier(NotifyProgress);
        }

        private void NotifyProgress(string message)
        {
            var progress = _manager.GetProgress();
            var header = $"Fi={progress.FinishedScenarios:D3},Fa={progress.FailedScenarios:D3},Pe={progress.PendingScenarios:D3} #{progress.CurrentScenarioNumber,3}> ";
            _onNotify(header + message.Replace(Environment.NewLine, Environment.NewLine + new string(' ', header.Length)));
        }

        //TODO: protect scenario finished from throwing exceptions
        public void Notify(ProgressEvent e)
        {
            switch (e)
            {
                case ScenarioFinished scenarioFinished:
                    _manager.CaptureScenarioResult(scenarioFinished.Result.Status);
                    _notifier.Notify(e);
                    _manager.FinishScenario();
                    break;
                case ScenarioStarting:
                    _manager.StartNewScenario();
                    _notifier.Notify(e);
                    break;
                default:
                    _notifier.Notify(e);
                    break;
            }
        }
    }
}