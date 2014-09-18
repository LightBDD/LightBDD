using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.Results.Implementation;

namespace LightBDD.Execution
{
    internal class ScenarioExecutor : IScenarioExecutor
    {
        private readonly IProgressNotifier _progressNotifier;
        public event Action<IScenarioResult> ScenarioExecuted;

        public ScenarioExecutor(IProgressNotifier progressNotifier)
        {
            _progressNotifier = progressNotifier;
        }

        public void Execute(string scenarioName, string label, IEnumerable<IStep> steps)
        {
            _progressNotifier.NotifyScenarioStart(scenarioName, label);
            var stepsToExecute = steps.ToArray();

            try
            {
                foreach (var step in stepsToExecute)
                    step.Invoke(_progressNotifier, stepsToExecute.Length);
            }
            finally
            {
                var result = new ScenarioResult(scenarioName, stepsToExecute.Select(s => s.GetResult()), label);
                if (ScenarioExecuted != null)
                    ScenarioExecuted.Invoke(result);
                _progressNotifier.NotifyScenarioFinished(result.Status, result.StatusDetails);
            }
        }
    }
}