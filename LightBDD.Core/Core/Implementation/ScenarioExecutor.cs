using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Execution.Results.Implementation;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Implementation
{
    internal class ScenarioExecutor
    {
        public async Task Execute(ScenarioInfo scenario, IEnumerable<RunnableStep> steps)
        {
            var preparedSteps = steps.ToArray();
            try
            {
                foreach (var step in preparedSteps)
                    await step.Invoke(null);
            }
            finally
            {
                Notify(new ScenarioResult(scenario, preparedSteps.Select(s => s.Result).ToArray()));
            }
        }

        private void Notify(ScenarioResult result)
        {
            var notifier = ScenarioExecuted;
            if (notifier != null)
                notifier.Invoke(result);
        }

        public event Action<IScenarioResult> ScenarioExecuted;
    }
}