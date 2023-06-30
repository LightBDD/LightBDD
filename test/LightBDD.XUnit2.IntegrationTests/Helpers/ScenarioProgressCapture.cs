using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;

namespace LightBDD.XUnit2.IntegrationTests.Helpers
{
    public class ScenarioProgressCapture : IProgressNotifier
    {
        private readonly ConcurrentQueue<IScenarioResult> _results = new();
        public IEnumerable<IScenarioResult> Results => _results;
        public static ScenarioProgressCapture Instance { get; } = new();

        public void Notify(ProgressEvent e)
        {
            if (e is ScenarioFinished sf)
                _results.Enqueue(sf.Result);
        }
    }
}