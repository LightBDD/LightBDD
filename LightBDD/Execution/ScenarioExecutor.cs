using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [DebuggerStepThrough]
        public void Execute(Scenario scenario, IEnumerable<IStep> steps)
        {
            _progressNotifier.NotifyScenarioStart(scenario.Name, scenario.Label);
            var stepsToExecute = steps.ToArray();

            var watch = new Stopwatch();
            var scenarioStartTime = DateTimeOffset.UtcNow;
            try
            {
                watch.Start();
                ExecuteSteps(stepsToExecute);
            }
            finally
            {
                watch.Stop();
                var result = new ScenarioResult(scenario.Name, stepsToExecute.Select(s => s.GetResult()), scenario.Label)
                .SetExecutionStart(scenarioStartTime)
                .SetExecutionTime(watch.Elapsed);

                if (ScenarioExecuted != null)
                    ScenarioExecuted.Invoke(result);

                _progressNotifier.NotifyScenarioFinished(result);
            }
        }

        private void ExecuteSteps(IStep[] stepsToExecute)
        {
            foreach (var step in stepsToExecute)
                step.Invoke(_progressNotifier, stepsToExecute.Length);
        }
    }
}