using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ScenarioExecutor
    {
        private readonly IProgressNotifier _progressNotifier;

        public ScenarioExecutor(IProgressNotifier progressNotifier)
        {
            _progressNotifier = progressNotifier;
        }

        public async Task Execute(ScenarioInfo scenario, IEnumerable<RunnableStep> steps)
        {
            var scenarioContext = new ScenarioContext();
            try
            {
                ScenarioContext.Current = scenarioContext;
                await ExecuteWithinSynchronizationContext(scenario, steps, scenarioContext);
            }
            finally
            {
                ScenarioContext.Current = null;
            }
        }

        private async Task ExecuteWithinSynchronizationContext(ScenarioInfo scenario, IEnumerable<RunnableStep> steps, ScenarioContext scenarioContext)
        {
            var preparedSteps = steps.ToArray();
            _progressNotifier.NotifyScenarioStart(scenario);

            var watch = ExecutionTimeWatch.StartNew();

            try
            {
                foreach (var step in preparedSteps)
                    await step.Invoke(scenarioContext, null);
            }
            finally
            {
                watch.Stop();

                var result = new ScenarioResult(
                    scenario,
                    preparedSteps.Select(s => s.Result).ToArray(),
                    watch.GetTime());

                _progressNotifier.NotifyScenarioFinished(result);
                ScenarioExecuted?.Invoke(result);
            }
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}