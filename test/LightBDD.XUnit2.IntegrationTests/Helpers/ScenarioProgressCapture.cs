using System.Collections.Concurrent;
using System.Collections.Generic;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.XUnit2.IntegrationTests.Helpers
{
    public class ScenarioProgressCapture : IScenarioProgressNotifier
    {
        private readonly ConcurrentQueue<IScenarioResult> _results = new ConcurrentQueue<IScenarioResult>();
        public IEnumerable<IScenarioResult> Results => _results;
        public static ScenarioProgressCapture Instance { get; } = new ScenarioProgressCapture();
        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
        }

        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            _results.Enqueue(scenario);
        }

        public void NotifyStepStart(IStepInfo step)
        {
        }

        public void NotifyStepFinished(IStepResult step)
        {
        }

        public void NotifyStepComment(IStepInfo step, string comment)
        {
        }
    }
}